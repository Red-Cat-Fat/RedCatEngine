using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs.UnityEditor
{
#if UNITY_EDITOR
	public class UnityEditorLogServiceWindow : EditorWindow
	{
		private Vector2 _scrollPosition;
		private string _searchFilter = "";

		[MenuItem("Tools/🐛 EditorLog")]
		public static void ShowWindow()
		{
			var window = GetWindow<UnityEditorLogServiceWindow>("🐛 Editor Log Settings");
			window.minSize = new Vector2(300, 400);
			UnityEditorLogServiceStaticBridge.LoadStates();
		}

		private void OnGUI()
		{
			DrawSearchField();
			DrawControlButtons();
			DrawTagList();
		}

		private void DrawSearchField()
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Log Filter Settings", EditorStyles.boldLabel);

			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Label("Search:", GUILayout.Width(50));
				_searchFilter = EditorGUILayout.TextField(_searchFilter);
			}

			EditorGUILayout.Space();
		}

		private void DrawControlButtons()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Select All"))
				{
					var filteredTags = UnityEditorLogServiceStaticBridge.AllTags
						.Where(tag => tag.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
						.OrderBy(tag => tag);

					foreach (var tag in filteredTags)
					{
						SetEnable(tag, true);
					}
				}

				if (GUILayout.Button("Deselect All"))
				{
					var filteredTags = UnityEditorLogServiceStaticBridge.AllTags
						.Where(tag => tag.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
						.OrderBy(tag => tag);

					foreach (var tag in filteredTags)
					{
						SetEnable(tag, false);
					}
				}
			}

			EditorGUILayout.Space();
		}

		private void DrawTagList()
		{
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

			var filteredTags = UnityEditorLogServiceStaticBridge.AllTags
				.Where(tag => tag.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)
				.OrderBy(tag => tag);

			HierarchicalTagDrawer.Draw(filteredTags);

			EditorGUILayout.EndScrollView();
		}

		private void SetEnable(string tag, bool newState)
		{
			var needChange = UnityEditorLogServiceStaticBridge.AllTags.Where(item => !string.IsNullOrEmpty(item)
				&& item.Contains(tag, StringComparison.OrdinalIgnoreCase)
			);
			foreach (var tagItem in needChange)
			{
				UnityEditorLogServiceStaticBridge.SetEnable(tagItem, newState);
			}
		}
	}
#endif
}