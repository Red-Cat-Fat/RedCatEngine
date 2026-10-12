using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs.Sinks
{
	/// <summary>
	/// Куда класть файлы логов и как их называть.
	/// Dev — рабочий стол (тестировщику проще найти), Release — persistentDataPath.
	/// </summary>
	public static class LogFileLocation
	{
		private const string LaunchPlayerNameArgument = "-name";
		private const string ParrelSyncCloneFileName = ".clone";
		private const string ParrelSyncArgumentFileName = ".parrelsyncarg";
		private const string DefaultEditorPlayerName = "MainEditor";

		public static string ResolveRootFolder(string folderName)
		{
#if UNITY_EDITOR || CHEAT_ENABLED || DEVELOPMENT_BUILD
			var root = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

			// На macOS/Linux в CI рабочего стола может не быть
			if(string.IsNullOrEmpty(root))
				root = Application.persistentDataPath;
#else
			var root = Path.Combine(Application.persistentDataPath, "Logs");
#endif
			return Path.Combine(root, folderName);
		}

		public static string BuildFileName(string filePrefix, DateTime time, int partIndex)
		{
			var timeStamp = time.ToString("MM-dd-yyyy_HH-mm");
			var part = partIndex > 1
					? $"_part{partIndex}"
					: string.Empty;

#if UNITY_EDITOR
			var prefix = (filePrefix ?? string.Empty).TrimEnd('_');
			var playerName = ResolveEditorPlayerName();
			return $"{prefix}_{timeStamp}_{playerName}{part}.log";
#else
			return $"{filePrefix}{timeStamp}{part}.log";
#endif
		}

#if UNITY_EDITOR
		private static string ResolveEditorPlayerName()
		{
			var playerName = TryGetLaunchArgumentPlayerName();
			if(string.IsNullOrWhiteSpace(playerName))
				playerName = TryGetParrelSyncArgumentPlayerName();

			if(string.IsNullOrWhiteSpace(playerName))
				playerName = DefaultEditorPlayerName;

			return SanitizeFileNamePart(playerName.Trim());
		}

		private static string TryGetLaunchArgumentPlayerName()
		{
			var arguments = Environment.GetCommandLineArgs();
			for(var i = 0; i < arguments.Length - 1; i++)
			{
				if(!string.Equals(arguments[i], LaunchPlayerNameArgument, StringComparison.OrdinalIgnoreCase))
					continue;

				var value = arguments[i + 1];
				if(string.IsNullOrWhiteSpace(value) || value.StartsWith("-", StringComparison.Ordinal))
					return null;

				return value;
			}

			return null;
		}

		private static string TryGetParrelSyncArgumentPlayerName()
		{
			try
			{
				var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
				var cloneMarkerPath = Path.Combine(projectRoot, ParrelSyncCloneFileName);
				if(!File.Exists(cloneMarkerPath))
					return null;

				var argumentFilePath = Path.Combine(projectRoot, ParrelSyncArgumentFileName);
				if(!File.Exists(argumentFilePath))
					return null;

				var argument = File.ReadAllText(argumentFilePath, Encoding.UTF8);
				return string.IsNullOrWhiteSpace(argument) ? null : argument.Trim();
			}
			catch(Exception)
			{
				return null;
			}
		}

		private static string SanitizeFileNamePart(string value)
		{
			var invalidChars = Path.GetInvalidFileNameChars();
			var builder = new StringBuilder(value.Length);
			foreach(var character in value)
			{
				if(Array.IndexOf(invalidChars, character) >= 0 || char.IsWhiteSpace(character))
					builder.Append('_');
				else
					builder.Append(character);
			}

			return builder.Length > 0 ? builder.ToString() : DefaultEditorPlayerName;
		}
#endif
	}
}
