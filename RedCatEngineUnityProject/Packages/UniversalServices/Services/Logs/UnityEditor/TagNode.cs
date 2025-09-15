using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace RedCatEngine.CommonServices.Services.Logs.UnityEditor
{
	/// <summary>
	/// Вспомогательный класс‑узел для построения иерархии тегов.
	/// </summary>
	internal sealed class TagNode
	{
		/// <summary>Имя узла (последняя часть пути).</summary>
		public string Name { get; }

		/// <summary>Полный путь, как в исходном списке.</summary>
		public string FullPath { get; }

		public int Depth { get; }

		/// <summary>Список дочерних узлов.</summary>
		public readonly List<TagNode> Children = new();

		public TagNode(string name, string fullPath)
		{
			Name = name;
			FullPath = fullPath;
			Depth = FullPath.Split(':').Length - 1;
		}
	}

	/// <summary>
	/// Класс‑помощник для работы с иерархией тегов в редакторе.
	/// </summary>
	internal static class HierarchicalTagDrawer
	{
#if UNITY_EDITOR
		/// <summary>
		/// Построить дерево из списка строк‑путей.
		/// </summary>
		private static TagNode BuildTree(IEnumerable<string> tags)
		{
			var root = new TagNode(string.Empty, string.Empty);

			foreach (var tag in tags)
			{
				// Разбиваем путь на части
				var parts = tag.Split(':');
				var current = root;
				string pathSoFar = string.Empty;

				foreach (var part in parts)
				{
					pathSoFar += (pathSoFar == string.Empty
							? ""
							: ":")
						+ part;

					// Найти существующий дочерний узел
					var child = current.Children.FirstOrDefault(c => c.FullPath == pathSoFar);
					if (child == null)
					{
						child = new TagNode(part, pathSoFar);
						current.Children.Add(child);
					}

					current = child;
				}
			}

			return root;
		}

		/// <summary>
		/// Сортировать дочерние узлы по имени (можно заменить на любой другой компаратор).
		/// </summary>
		private static void SortChildren(TagNode node)
		{
			node.Children.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
			foreach (var child in node.Children)
				SortChildren(child);
		}

		/// <summary>
		/// Получить упорядоченный список тегов с уровнем вложенности.
		/// </summary>
		private static IEnumerable<(string FullPath, int Depth)> GetOrderedTags(TagNode root)
		{
			var result = new List<(string, int)>();

			void Traverse(TagNode node)
			{
				if (!string.IsNullOrEmpty(node.FullPath))
					result.Add((node.FullPath, node.Depth));

				foreach (var child in node.Children)
					Traverse(child);
			}

			Traverse(root);
			return result;
		}

		/// <summary>
		/// Отрисовать список тегов в редакторе с отступами.
		/// </summary>
		public static void Draw(IEnumerable<string> tags)
		{
			// 1. Построить дерево
			var root = BuildTree(tags);

			// 2. Сортировать узлы
			SortChildren(root);

			// 3. Получить упорядоченный список (Depth – количество ':' в пути)
			var orderedTags = GetOrderedTags(root).ToList();

			// 4. Рисуем каждый тег
			foreach (var (fullPath, depth) in orderedTags)
			{
				EditorGUI.indentLevel = depth; // Устанавливаем отступ

				bool isEnabled = UnityEditorLogServiceStaticBridge.IsLogTypeEnabled(fullPath);
				bool newState = EditorGUILayout.ToggleLeft(fullPath, isEnabled);

				if (newState != isEnabled)
					SetEnable(fullPath, newState);
			}

			// Сбросить indentLevel в случае, если дальше рисуется что‑то ещё
			EditorGUI.indentLevel = 0;
		}

		private static void SetEnable(string tag, bool newState)
		{
			var needChange = UnityEditorLogServiceStaticBridge.AllTags.Where(
				item => !string.IsNullOrEmpty(item)
					&& item.Contains(tag, StringComparison.OrdinalIgnoreCase)
			);
			foreach (var tagItem in needChange)
			{
				UnityEditorLogServiceStaticBridge.SetEnable(tagItem, newState);
			}
		}
#endif
	}
}