using System.Collections.Generic;
using RedCatEngine.Configs;
using UnityEditor;
using UnityEngine;

namespace RedCatEngine.Quests.Configs.Quests
{
	[CreateAssetMenu(
		fileName = nameof(AllQuestLinksConfig),
		menuName = "Configs/Quests/Quest Systems/AllQuestCollection",
		order = 0)]
	public class AllQuestLinksConfig : BaseConfig
	{
		public List<QuestConfig> Quests = new();

#if UNITY_EDITOR
		protected override void DoValidate()
		{
			var quests = new List<QuestConfig>();
			var guids = AssetDatabase.FindAssets("t:" + nameof(QuestConfig));
			for (var i = 0; i < guids.Length; i++)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
				var asset = AssetDatabase.LoadAssetAtPath<QuestConfig>(assetPath);
				if (asset != null)
				{
					quests.Add(asset);
				}
			}

			if (quests.Count == Quests.Count)
				return;
			Quests.Clear();
			Quests = quests;
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}
#endif
	}
}