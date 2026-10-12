using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs.Editor
{
	/// <summary>
	/// Управление уровнями логирования по дереву тегов.
	/// Работает с живым фильтром из LogHub — и в Play Mode, и в edit-time: вне игры LogHub
	/// поднимает редакторный инсталлер, чтобы были видны сообщения из OnValidate.
	/// Список тегов при этом показывает только те, что уже создавались в текущей сессии.
	/// </summary>
	public sealed class LogFilterWindow : EditorWindow
	{
		private Vector2 _scrollPosition;
		private string _searchFilter = string.Empty;

		private void OnGUI()
		{
			var filter = LogHub.Filter;

			if(!LogHub.IsInitialized)
			{
				EditorGUILayout.HelpBox(
						"Система логирования не инициализирована. Перезапустите компиляцию или войдите в Play Mode.",
						MessageType.Info
				);
				return;
			}

			DrawGlobalLevel(filter);
			DrawSearchField();
			DrawControlButtons(filter);
			DrawTagList(filter);
		}

		private void OnInspectorUpdate()
			=> Repaint();

		[MenuItem("Tools/🐛 Log Filter")]
		public static void ShowWindow()
		{
			var window = GetWindow<LogFilterWindow>("🐛 Log Filter");
			window.minSize = new Vector2(420, 400);
		}

		private static void DrawGlobalLevel(Filtering.ILogLevelFilter filter)
		{
			EditorGUILayout.Space();

			using(new EditorGUILayout.HorizontalScope())
			{
				var selected = (LogLevel)EditorGUILayout.EnumPopup("Глобальный уровень", filter.GlobalLevel);
				if(selected != filter.GlobalLevel)
					filter.SetGlobalLevel(selected);
			}

			EditorGUILayout.LabelField($"Оверрайдов: {filter.Overrides.Count}", EditorStyles.miniLabel);
			EditorGUILayout.Space();
		}

		private void DrawSearchField()
		{
			using(new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Label("Поиск:", GUILayout.Width(50));
				_searchFilter = EditorGUILayout.TextField(_searchFilter);
			}

			EditorGUILayout.Space();
		}

		private void DrawControlButtons(Filtering.ILogLevelFilter filter)
		{
			using(new EditorGUILayout.HorizontalScope())
			{
				if(GUILayout.Button("Сбросить фильтр"))
					filter.Reset();

				if(GUILayout.Button("Сохранить"))
					filter.Save();

				if(GUILayout.Button("Снять оверрайды"))
				{
					foreach(var tag in filter.Overrides.Keys.ToArray())
						filter.ClearLevel(tag);
				}
			}

			EditorGUILayout.Space();
		}

		private void DrawTagList(Filtering.ILogLevelFilter filter)
		{
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

			var tags = GetFilteredTags(filter);

			HierarchicalTagDrawer.Draw(
					tags,
					tag => filter.Overrides.TryGetValue(tag, out var level) ? level : (LogLevel?)null,
					filter.Resolve,
					(tag, level) =>
					{
						if(level.HasValue)
							filter.SetLevel(tag, level.Value);
						else
							filter.ClearLevel(tag);
					}
			);

			EditorGUILayout.EndScrollView();
		}

		private IEnumerable<string> GetFilteredTags(Filtering.ILogLevelFilter filter)
		{
			var tags = new HashSet<string>(filter.KnownTags);
			foreach(var tag in filter.Overrides.Keys)
				tags.Add(tag);

			return tags
					.Where(tag => tag.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
					.OrderBy(tag => tag, StringComparer.Ordinal);
		}
	}
}
