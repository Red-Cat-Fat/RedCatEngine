using System.Collections.Generic;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Exceptions;
using UnityEditor;
using UnityEngine;

namespace RedCatEngine.DependencyInjection.Editor
{
	#if UNITY_EDITOR
	public class ContainerHierarchyWindow : EditorWindow
	{
		public static IApplicationContainer RootContainer;
		private readonly Dictionary<IApplicationContainer, bool> _foldouts = new();
		private Vector2 _scrollPosition;
		private string _searchQuery = "";

		[MenuItem("Tools/RedCat DI/Container Hierarchy")]
		public static void ShowWindow()
		{
			GetWindow<ContainerHierarchyWindow>("Container Hierarchy");
		}

		private void OnGUI()
		{
			GUILayout.Label("Root Container:", EditorStyles.boldLabel);
			if (RootContainer == null)
			{
				EditorGUILayout.HelpBox("Please assign a root container.", MessageType.Info);
				return;
			}

			// Поле ввода для поиска
			_searchQuery = EditorGUILayout.TextField("Search by key name:", _searchQuery);

			EditorGUILayout.Space();

			_foldouts.TryAdd(RootContainer, true);
			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
			{
				DrawContainerHierarchy(RootContainer, 0);
			}
			GUILayout.EndScrollView();
		}

		private void DrawContainerHierarchy(IApplicationContainer container, int indentLevel)
		{
			const float lineHeight = 18f;
			const float indentPerLevel = 20f;

			var labelWidth = EditorGUIUtility.labelWidth;
			var currentIndent = indentLevel * indentPerLevel;

			// Вычисляем доступную ширину
			var rect = GUILayoutUtility.GetRect(1f, lineHeight, GUILayout.ExpandWidth(true));
			rect.xMin += currentIndent; // Применяем отступ

			_foldouts.TryAdd(container, true);
			var displayText = $"Container: {container.Name} (Objects: {container.Objects.Count})";

			// Используем PrefixLabel, чтобы корректно отобразить текст с отступом
			var foldoutRect = new Rect(rect.x, rect.y, rect.width, rect.height);
			_foldouts[container] = EditorGUI.Foldout(foldoutRect, _foldouts[container], displayText, true);

			// Сдвигаем y для следующих элементов
			float nextY = rect.y + lineHeight;

			if (_foldouts[container])
			{
				// Отображаем объекты в контейнере
				foreach (var kvp in container.Objects)
				{
					var keyShortName = kvp.Key.ShortName();
					if (!string.IsNullOrEmpty(_searchQuery)
						&& !keyShortName.Contains(_searchQuery, System.StringComparison.OrdinalIgnoreCase))
						continue;

					var objectRect = GUILayoutUtility.GetRect(1f, lineHeight, GUILayout.ExpandWidth(true));
					objectRect.xMin += currentIndent + 10; // Доп. отступ для объектов
					GUI.Label(objectRect, $"- {keyShortName} -> {kvp.Value.GetType().ShortName()}");
				}

				// Рекурсивно отображаем дочерние контейнеры
				foreach (var child in container.ChildContainers)
				{
					DrawContainerHierarchy(child, indentLevel + 1);
				}
			}

			// Корректируем высоту, если используется скролл
			GUILayout.Space(nextY - rect.y - lineHeight);
		}
	}
	#endif
}