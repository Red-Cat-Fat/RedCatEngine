#if UNITY_EDITOR
using UnityEditor;

namespace RedCatEngine.CommonServices.Services.Logs.Filtering
{
	/// <summary>
	/// Персист фильтра в редакторе. Лежит в рантайм-сборке (а не в Editor-asmdef) намеренно:
	/// хранилище выбирает LogSystemInstaller из Assembly-CSharp, которому Editor-сборка недоступна.
	/// Весь файл под #if UNITY_EDITOR, включая using.
	///
	/// Ключи новые: старые EditorLogSettings* хранили список включённых тегов, а не уровни —
	/// семантика другая, миграция состояния не нужна.
	/// </summary>
	public sealed class EditorPrefsLogFilterStorage : ILogFilterStorage
	{
		private const string GlobalKey = "RedCatEngine.Log.Global";
		private const string OverridesKey = "RedCatEngine.Log.Overrides";

		public bool TryLoad(out string globalLevel, out string overrides)
		{
			globalLevel = null;
			overrides = null;

			if(!EditorPrefs.HasKey(GlobalKey))
				return false;

			globalLevel = EditorPrefs.GetString(GlobalKey, string.Empty);
			overrides = EditorPrefs.GetString(OverridesKey, string.Empty);
			return true;
		}

		public void Save(string globalLevel, string overrides)
		{
			EditorPrefs.SetString(GlobalKey, globalLevel ?? string.Empty);
			EditorPrefs.SetString(OverridesKey, overrides ?? string.Empty);
		}

		public void Clear()
		{
			EditorPrefs.DeleteKey(GlobalKey);
			EditorPrefs.DeleteKey(OverridesKey);
		}
	}
}
#endif
