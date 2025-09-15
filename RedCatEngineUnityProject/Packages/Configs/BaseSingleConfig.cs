using UnityEngine;

namespace RedCatEngine.Configs
{
	public class BaseSingleConfig<TConfig> : BaseConfig where TConfig : BaseConfig
	{
		private static TConfig _instance;

		public static TConfig Instance
		{
			get
			{
				if (_instance != null)
					return _instance;
				_instance = Resources.Load<TConfig>(nameof(TConfig));

#if UNITY_EDITOR
				if (_instance != null)
					return _instance;

				_instance = CreateInstance<TConfig>();
				UnityEditor.AssetDatabase.CreateAsset(
					_instance,
					$"Assets/Resources/Configs/{nameof(TConfig)}.asset");
				UnityEditor.AssetDatabase.SaveAssets();
#endif
				return _instance;
			}
		}

		protected override void DoValidate()
		{
			var otherConfigs = Resources.FindObjectsOfTypeAll<TConfig>();
			if (otherConfigs.Length > 1)
			{
				Debug.LogError("There are more than one instance of " + nameof(TConfig));
			}
		}
	}
}