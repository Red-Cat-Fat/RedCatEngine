using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs.Editor
{
	/// <summary>
	/// Узел иерархии тегов. Полный путь разделён двоеточиями.
	/// </summary>
	internal sealed class TagNode
	{
		public readonly List<TagNode> Children = new();

		public TagNode(string name, string fullPath)
		{
			Name = name;
			FullPath = fullPath;
			Depth = FullPath.Split(':').Length - 1;
		}

		public string Name { get; }
		public string FullPath { get; }
		public int Depth { get; }
	}

	/// <summary>
	/// Рисует дерево тегов с выпадающим списком уровня.
	/// Каскад по поддереву не нужен: оверрайд на узле действует на всех потомков
	/// через префиксный резолв фильтра.
	/// </summary>
	internal static class HierarchicalTagDrawer
	{
		public static void Draw(
				IEnumerable<string> tags,
				Func<string, LogLevel?> getOverride,
				Func<string, LogLevel> getEffective,
				Action<string, LogLevel?> setOverride
		)
		{
			var root = BuildTree(tags);
			SortChildren(root);

			foreach(var (fullPath, depth) in GetOrderedTags(root))
			{
				EditorGUI.indentLevel = depth;

				using(new EditorGUILayout.HorizontalScope())
				{
					var current = getOverride(fullPath);
					var effective = getEffective(fullPath);

					var displayed = current ?? effective;
					var selected = (LogLevel)EditorGUILayout.EnumPopup(fullPath, displayed);

					if(selected != displayed)
						setOverride(fullPath, selected);

					if(current == null)
					{
						EditorGUI.indentLevel = 0;
						GUILayout.Label($"(наследует: {effective})", EditorStyles.miniLabel, GUILayout.Width(140));
					}
					else if(GUILayout.Button("×", EditorStyles.miniButton, GUILayout.Width(22)))
					{
						setOverride(fullPath, null);
					}
				}
			}

			EditorGUI.indentLevel = 0;
		}

		private static TagNode BuildTree(IEnumerable<string> tags)
		{
			var root = new TagNode(string.Empty, string.Empty);
			var index = new Dictionary<string, TagNode>();

			foreach(var tag in tags)
			{
				var parts = tag.Split(':');
				var current = root;
				var pathSoFar = string.Empty;

				foreach(var part in parts)
				{
					pathSoFar += pathSoFar.Length == 0
							? part
							: ":" + part;

					if(!index.TryGetValue(pathSoFar, out var child))
					{
						child = new TagNode(part, pathSoFar);
						index[pathSoFar] = child;
						current.Children.Add(child);
					}

					current = child;
				}
			}

			return root;
		}

		private static void SortChildren(TagNode node)
		{
			node.Children.Sort((left, right) => string.CompareOrdinal(left.Name, right.Name));
			foreach(var child in node.Children)
				SortChildren(child);
		}

		private static IEnumerable<(string FullPath, int Depth)> GetOrderedTags(TagNode root)
		{
			var result = new List<(string, int)>();

			void Traverse(TagNode node)
			{
				if(!string.IsNullOrEmpty(node.FullPath))
					result.Add((node.FullPath, node.Depth));

				foreach(var child in node.Children)
					Traverse(child);
			}

			Traverse(root);
			return result;
		}
	}
}
