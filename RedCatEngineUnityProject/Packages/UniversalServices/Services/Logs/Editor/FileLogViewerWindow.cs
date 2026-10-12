using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs.Editor
{
	/// <summary>
	/// Просмотр файлов, которые пишет FileLogSink.
	/// Формат строки: [HH:mm:ss.fff] [LEVEL] [Tnn] [Tag] message
	/// </summary>
	public sealed class FileLogViewerWindow : EditorWindow
	{
		private const int MaxDisplayedLines = 5000;

		private static readonly string[] LevelTokens = { "[VERB]", "[INFO]", "[WARN]", "[ERROR]" };

		private readonly List<string> _allLines = new();
		private readonly List<string> _filteredLines = new();
		private bool _autoScrollToBottom;

		private bool _ignoreCase = true;

		private string _loadedFilePath;
		private LogLevel _minLevel = LogLevel.Verbose;

		private Vector2 _scrollPosition;

		private string _searchText = string.Empty;
		private bool _showLineNumbers = true;
		private string _tagFilter = string.Empty;

		private void OnGUI()
		{
			DrawToolbar();
			EditorGUILayout.Space(5);
			DrawFilters();
			EditorGUILayout.Space(5);
			DrawInfo();
			EditorGUILayout.Space(5);
			DrawLogs();
		}

		[MenuItem("Tools/Log Viewer")]
		private static void Open()
		{
			var window = GetWindow<FileLogViewerWindow>();

			window.titleContent = new GUIContent("Log Viewer");
			window.minSize = new Vector2(800, 500);

			window.Show();
		}

		private void DrawToolbar()
		{
			using(new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				if(GUILayout.Button("Open Log", EditorStyles.toolbarButton, GUILayout.Width(90)))
					OpenLogFile();

				using(new EditorGUI.DisabledScope(string.IsNullOrEmpty(LogHub.FilePath?.CurrentLogFilePath)))
				{
					if(GUILayout.Button("Open Current Session", EditorStyles.toolbarButton, GUILayout.Width(160)))
						LoadFile(LogHub.FilePath.CurrentLogFilePath);
				}

				if(GUILayout.Button("Reload", EditorStyles.toolbarButton, GUILayout.Width(70)))
					LoadFile(_loadedFilePath);

				GUILayout.Space(10);

				GUILayout.Label("Поиск:", GUILayout.Width(45));

				var newSearch = GUILayout.TextField(_searchText, EditorStyles.toolbarTextField, GUILayout.Width(260));
				if(newSearch != _searchText)
				{
					_searchText = newSearch;
					ApplyFilter();
				}

				GUILayout.FlexibleSpace();

				if(GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(70)))
					Clear();
			}
		}

		private void DrawFilters()
		{
			using(new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Label("Уровень от:", GUILayout.Width(80));

				var newLevel = (LogLevel)EditorGUILayout.EnumPopup(_minLevel, GUILayout.Width(100));
				if(newLevel != _minLevel)
				{
					_minLevel = newLevel;
					ApplyFilter();
				}

				GUILayout.Space(10);
				GUILayout.Label("Тег:", GUILayout.Width(30));

				var newTag = GUILayout.TextField(_tagFilter, GUILayout.Width(200));
				if(newTag != _tagFilter)
				{
					_tagFilter = newTag;
					ApplyFilter();
				}

				GUILayout.Space(10);

				var newIgnoreCase = GUILayout.Toggle(_ignoreCase, "Ignore Case", GUILayout.Width(100));
				if(newIgnoreCase != _ignoreCase)
				{
					_ignoreCase = newIgnoreCase;
					ApplyFilter();
				}

				_showLineNumbers = GUILayout.Toggle(_showLineNumbers, "Line Numbers", GUILayout.Width(110));
				_autoScrollToBottom = GUILayout.Toggle(_autoScrollToBottom, "Auto Scroll", GUILayout.Width(100));
			}
		}

		private void DrawInfo()
		{
			EditorGUILayout.LabelField($"Loaded File: {_loadedFilePath}", EditorStyles.helpBox);
			EditorGUILayout.LabelField(
					$"Total Lines: {_allLines.Count} | Filtered: {_filteredLines.Count}",
					EditorStyles.helpBox
			);
		}

		private void DrawLogs()
		{
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

			for(var i = 0; i < _filteredLines.Count; i++)
				DrawLogLine(i, _filteredLines[i]);

			EditorGUILayout.EndScrollView();

			if(_autoScrollToBottom && Event.current.type == EventType.Repaint)
				_scrollPosition.y = float.MaxValue;
		}

		private void DrawLogLine(int index, string line)
		{
			var style = GetStyleForLine(line);

			using(new EditorGUILayout.HorizontalScope())
			{
				if(_showLineNumbers)
					GUILayout.Label(index.ToString(), GUILayout.Width(60));

				EditorGUILayout.SelectableLabel(line, style, GUILayout.Height(18));
			}
		}

		private static GUIStyle GetStyleForLine(string line)
		{
			var style = new GUIStyle(EditorStyles.label)
			{
					richText = true
			};

			if(line.Contains("[ERROR]"))
				style.normal.textColor = Color.red;
			else if(line.Contains("[WARN]"))
				style.normal.textColor = new Color(1f, 0.65f, 0f);
			else if(line.Contains("[VERB]"))
				style.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
			else
				style.normal.textColor = Color.white;

			return style;
		}

		private void OpenLogFile()
		{
			var startFolder = string.IsNullOrEmpty(LogHub.FilePath?.CurrentLogFilePath)
					? string.Empty
					: Path.GetDirectoryName(LogHub.FilePath.CurrentLogFilePath);

			var path = EditorUtility.OpenFilePanel("Open Log File", startFolder ?? string.Empty, "log,txt");
			if(string.IsNullOrWhiteSpace(path))
				return;

			LoadFile(path);
		}

		private void LoadFile(string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				return;

			try
			{
				_loadedFilePath = path;
				_allLines.Clear();

				// FileShare.ReadWrite — файл держит открытым живой писатель игры
				using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				using var reader = new StreamReader(stream);

				while(reader.ReadLine() is { } line)
					_allLines.Add(line);

				ApplyFilter();
			}
			catch(Exception exception)
			{
				Debug.LogError($"Failed load log file: {exception}");
			}
		}

		private void ApplyFilter()
		{
			_filteredLines.Clear();

			var comparison = _ignoreCase
					? StringComparison.OrdinalIgnoreCase
					: StringComparison.Ordinal;

			var searchTokens = _searchText
					.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(token => token.Trim())
					.Where(token => !string.IsNullOrWhiteSpace(token))
					.ToArray();

			var hasTagFilter = !string.IsNullOrWhiteSpace(_tagFilter);

			foreach(var line in _allLines)
			{
				if(!MatchesLevel(line))
					continue;

				if(hasTagFilter && line.IndexOf(_tagFilter, comparison) < 0)
					continue;

				var containsAllTokens = true;
				foreach(var token in searchTokens)
				{
					if(line.IndexOf(token, comparison) >= 0)
						continue;

					containsAllTokens = false;
					break;
				}

				if(containsAllTokens)
					_filteredLines.Add(line);
			}

			if(_filteredLines.Count > MaxDisplayedLines)
				_filteredLines.RemoveRange(0, _filteredLines.Count - MaxDisplayedLines);
		}

		private bool MatchesLevel(string line)
		{
			if(_minLevel == LogLevel.Verbose)
				return true;

			for(var level = 0; level < LevelTokens.Length; level++)
			{
				if(!line.Contains(LevelTokens[level]))
					continue;

				return level >= (int)_minLevel;
			}

			// Строки продолжения (стектрейсы) не содержат токен уровня — показываем их всегда
			return true;
		}

		private void Clear()
		{
			_allLines.Clear();
			_filteredLines.Clear();
			_loadedFilePath = string.Empty;
		}
	}
}
