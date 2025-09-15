using System;
using System.Collections.Generic;
using UnityEditor;

namespace RedCatEngine.CommonServices.Services.Logs.UnityEditor
{
#if UNITY_EDITOR
	public static class UnityEditorLogServiceStaticBridge
	{
		public static readonly HashSet<string> AllTags = new();
		private static readonly HashSet<string> ShowTags = new();
		private const string PrefsKeyAll = "EditorLogSettingsAllTags";
		private const string PrefsKeyShow = "EditorLogSettingsShowTags";

		public static bool IsLogTypeEnabled(string logType)
			=> ShowTags.Contains(logType);

		public static void AddLog(string logType)
		{
			if (!AllTags.Add(logType))
				return;
			ShowTags.Add(logType);
		}

		public static void SetEnable(string logType, bool enable)
		{
			AllTags.Add(logType);

			if (enable)
				ShowTags.Add(logType);
			else
				ShowTags.Remove(logType);

			SaveStates();
		}

		public static void SetAllEnabled(bool enabled)
		{
			ShowTags.Clear();

			if (enabled)
			{
				foreach (var tag in AllTags)
				{
					ShowTags.Add(tag);
				}
			}

			SaveStates();
		}

		private static void SaveStates()
		{
			var data = string.Join(";", ShowTags);
			EditorPrefs.SetString(PrefsKeyShow, data);
			data = string.Join(";", AllTags);
			EditorPrefs.SetString(PrefsKeyAll, data);
		}

		public static void LoadStates()
		{
			if (!EditorPrefs.HasKey(PrefsKeyAll))
				return;

			var allData = EditorPrefs.GetString(PrefsKeyAll, "");
			if (string.IsNullOrEmpty(allData))
				return;

			AllTags.Clear();
			var allTags = allData.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var tag in allTags)
				AllTags.Add(tag);

			var showData = EditorPrefs.GetString(PrefsKeyAll, "");
			if (string.IsNullOrEmpty(showData))
				return;

			ShowTags.Clear();
			var showTags = showData.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var tag in showTags)
				ShowTags.Add(tag);
		}
	}
#endif
}