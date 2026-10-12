using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RedCatEngine.CommonServices.Services.Logs.Filtering
{
	/// <summary>
	/// Иерархический фильтр уровней. Резолв поднимается по тегу до первого явного оверрайда:
	/// "Npc:Wolf:Brain" → "Npc:Wolf" → "Npc" → GlobalLevel.
	///
	/// Resolve вызывается с любых потоков (Wwise, Mirror, Job System, logMessageReceivedThreaded),
	/// поэтому кэш и реестр тегов — конкурентные коллекции, а Version читается через volatile.
	/// Мутирующие методы вызываются с главного потока (консоль, редакторное окно).
	/// </summary>
	public sealed class LogLevelFilter : ILogLevelFilter
	{
		/// <summary>
		/// Потолок реестра структурных тегов. Защита от регистрации инстансных тегов
		/// (по одному на каждый заспавненный объект) — см. CreateInstanceTag.
		/// </summary>
		private const int MaxKnownTags = 2000;

		/// <summary>Потолок кэша резолва: инстансные теги в реестр не попадают, но кэшируются.</summary>
		private const int MaxResolveCacheEntries = 8000;

		private const char TagSeparator = ':';
		private const char OverrideSeparator = ';';
		private const char OverrideAssign = '=';
		private readonly LogLevel _defaultGlobalLevel;
		private readonly KeyValuePair<string, LogLevel>[] _defaultOverrides;
		private readonly ConcurrentDictionary<string, byte> _knownTags = new();

		private readonly ConcurrentDictionary<string, LogLevel> _overrides = new();
		private readonly ConcurrentDictionary<string, LogLevel> _resolveCache = new();

		private readonly ILogFilterStorage _storage;
		private volatile int _globalLevel;

		private bool _isTagRegistryOverflowReported;
		private volatile int _version;

		public LogLevelFilter(
				ILogFilterStorage storage,
				LogLevel defaultGlobalLevel,
				KeyValuePair<string, LogLevel>[] defaultOverrides = null
		)
		{
			_storage = storage ?? new NullLogFilterStorage();
			_defaultGlobalLevel = defaultGlobalLevel;
			_defaultOverrides = defaultOverrides ?? Array.Empty<KeyValuePair<string, LogLevel>>();

			ApplyDefaults();
			LoadFromStorage();
		}

		public int Version => _version;

		public LogLevel GlobalLevel
		{
			get => (LogLevel)_globalLevel;
			private set => _globalLevel = (int)value;
		}

		public IReadOnlyCollection<string> KnownTags => (IReadOnlyCollection<string>)_knownTags.Keys;
		public IReadOnlyDictionary<string, LogLevel> Overrides => _overrides;

		public LogLevel Resolve(string tag)
		{
			if(string.IsNullOrEmpty(tag))
				return GlobalLevel;

			if(_resolveCache.TryGetValue(tag, out var cached))
				return cached;

			var level = GlobalLevel;
			var probe = tag;

			while(true)
			{
				if(_overrides.TryGetValue(probe, out var explicitLevel))
				{
					level = explicitLevel;
					break;
				}

				var separator = probe.LastIndexOf(TagSeparator);
				if(separator < 0)
					break;

				probe = probe.Substring(0, separator);
			}

			if(_resolveCache.Count >= MaxResolveCacheEntries)
				_resolveCache.Clear();

			_resolveCache[tag] = level;
			return level;
		}

		public void SetGlobalLevel(LogLevel level)
		{
			if(GlobalLevel == level)
				return;

			GlobalLevel = level;
			Invalidate();
		}

		public void SetLevel(string tagPrefix, LogLevel level)
		{
			if(string.IsNullOrEmpty(tagPrefix))
			{
				SetGlobalLevel(level);
				return;
			}

			if(_overrides.TryGetValue(tagPrefix, out var current) && current == level)
				return;

			_overrides[tagPrefix] = level;
			Invalidate();
		}

		public void ClearLevel(string tagPrefix)
		{
			if(string.IsNullOrEmpty(tagPrefix))
				return;

			if(!_overrides.TryRemove(tagPrefix, out _))
				return;

			Invalidate();
		}

		public void Reset()
		{
			_storage.Clear();
			ApplyDefaults();
			Invalidate();
		}

		public void Save()
			=> _storage.Save(GlobalLevel.ToString(), SerializeOverrides());

		public void RegisterTag(string tag)
		{
			if(string.IsNullOrEmpty(tag))
				return;

			if(_knownTags.ContainsKey(tag))
				return;

			if(_knownTags.Count >= MaxKnownTags)
			{
				ReportTagRegistryOverflow(tag);
				return;
			}

			_knownTags.TryAdd(tag, 0);
		}

		private void ApplyDefaults()
		{
			GlobalLevel = _defaultGlobalLevel;

			_overrides.Clear();
			foreach(var pair in _defaultOverrides)
				_overrides[pair.Key] = pair.Value;
		}

		private void LoadFromStorage()
		{
			if(!_storage.TryLoad(out var globalLevel, out var overrides))
				return;

			if(TryParseLevel(globalLevel, out var parsedGlobal))
				GlobalLevel = parsedGlobal;

			DeserializeOverrides(overrides);
			Invalidate();
		}

		private string SerializeOverrides()
		{
			if(_overrides.Count == 0)
				return string.Empty;

			var builder = new StringBuilder();
			foreach(var pair in _overrides)
			{
				if(builder.Length > 0)
					builder.Append(OverrideSeparator);

				builder.Append(pair.Key).Append(OverrideAssign).Append(pair.Value);
			}

			return builder.ToString();
		}

		private void DeserializeOverrides(string overrides)
		{
			if(string.IsNullOrEmpty(overrides))
				return;

			// Дефолты не стираем: сохранённые значения только перекрывают их поверх
			var entries = overrides.Split(new[] { OverrideSeparator }, StringSplitOptions.RemoveEmptyEntries);
			foreach(var entry in entries)
			{
				var assign = entry.IndexOf(OverrideAssign);
				if(assign <= 0)
					continue;

				var tag = entry.Substring(0, assign);
				var levelText = entry.Substring(assign + 1);

				if(TryParseLevel(levelText, out var level))
					_overrides[tag] = level;
			}
		}

		private static bool TryParseLevel(string text, out LogLevel level)
		{
			// Enum.TryParse принимает числа, поэтому проверяем принадлежность перечислению
			if(Enum.TryParse(text, true, out level) && Enum.IsDefined(typeof(LogLevel), level))
				return true;

			level = default;
			return false;
		}

		private void ReportTagRegistryOverflow(string tag)
		{
			if(_isTagRegistryOverflowReported)
				return;

			_isTagRegistryOverflowReported = true;
			UnityEngine.Debug.LogWarning(
					$"[Log] Реестр тегов переполнен ({MaxKnownTags}). "
					+ $"Вероятно инстансный тег зарегистрирован как структурный: '{tag}'. "
					+ "Используйте CreateInstanceTag для тегов, зависящих от конкретного объекта."
			);
		}

		private void Invalidate()
		{
			_resolveCache.Clear();
			Interlocked.Increment(ref _version);
		}
	}
}
