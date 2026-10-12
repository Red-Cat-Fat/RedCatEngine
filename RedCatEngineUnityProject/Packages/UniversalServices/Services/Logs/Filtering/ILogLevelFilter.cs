using System.Collections.Generic;

namespace RedCatEngine.CommonServices.Services.Logs.Filtering
{
	/// <summary>
	/// Резолвит эффективный уровень логирования для тега.
	/// Теги иерархические, разделитель — ':' ("Npc:Wolf:Brain").
	/// </summary>
	public interface ILogLevelFilter
	{
		/// <summary>
		/// Монотонный счётчик. Инкрементируется при любом изменении фильтра,
		/// потребители кэшируют resolved-уровень и сравнивают версию.
		/// </summary>
		int Version { get; }

		LogLevel GlobalLevel { get; }

		IReadOnlyCollection<string> KnownTags { get; }
		IReadOnlyDictionary<string, LogLevel> Overrides { get; }

		/// <summary>
		/// Побеждает самый длинный явно заданный префикс, иначе <see cref="GlobalLevel"/>.
		/// </summary>
		LogLevel Resolve(string tag);

		void SetGlobalLevel(LogLevel level);
		void SetLevel(string tagPrefix, LogLevel level);
		void ClearLevel(string tagPrefix);

		/// <summary>Вернуться к дефолтам конфигурации.</summary>
		void Reset();

		/// <summary>Сохранить текущее состояние в хранилище.</summary>
		void Save();

		/// <summary>Зарегистрировать структурный тег (для списка в консоли и редакторном окне).</summary>
		void RegisterTag(string tag);
	}
}
