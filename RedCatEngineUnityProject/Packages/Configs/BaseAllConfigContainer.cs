#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
#endif
using UnityEngine;

namespace RedCatEngine.Configs
{
	public abstract class BaseAllConfigContainerConfig<TConfig> : BaseConfig, IAllConfigContainer<TConfig>
			where TConfig : BaseConfig
	{
		[SerializeField] protected TConfig[] _allConfigs;
		public TConfig[] AllConfigs => _allConfigs;

		public bool TryGetById(uint configId, out TConfig tag)
		{
			if(AllConfigs == null)
			{
				tag = null;
				return false;
			}

			foreach(var config in AllConfigs)
			{
				if(config == null || config.Id != configId)
					continue;
				tag = config;
				return true;
			}

			tag = null;
			return false;
		}

#if UNITY_EDITOR
		public void CollectLinks()
		{
			var collectedConfigs = new List<TConfig>();

			var guids = AssetDatabase.FindAssets($"t:{typeof(TConfig).Name}");

			foreach(var guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var config = AssetDatabase.LoadAssetAtPath<TConfig>(path);

				if(config == null)
					continue;

				collectedConfigs.Add(config);
			}

			_allConfigs = collectedConfigs
					.OrderBy(c => c.Id)
					.ToArray();

			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}
#endif
	}
}
