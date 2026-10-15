using UnityEngine;

namespace RedCatEngine.Configs
{
	/// <summary>
	/// A config with a single asset in the project, available through <see cref="Instance"/>.
	/// The asset is looked up in <c>Resources/Configs/&lt;TypeName&gt;</c>, then in <c>Resources/&lt;TypeName&gt;</c>.
	/// In the Editor a missing asset is created at <c>Assets/Resources/Configs/&lt;TypeName&gt;.asset</c>.
	/// </summary>
	public class BaseSingleConfig<TConfig> : BaseConfig where TConfig : BaseConfig
	{
		private const string ResourcesFolder = "Configs";

		private static TConfig _instance;

		private static string AssetName
			=> typeof(TConfig).Name;

		public static TConfig Instance
		{
			get
			{
				if (_instance != null)
					return _instance;

				_instance = Resources.Load<TConfig>($"{ResourcesFolder}/{AssetName}")
					?? Resources.Load<TConfig>(AssetName);

#if UNITY_EDITOR
				if (_instance != null)
					return _instance;

				_instance = CreateInstance<TConfig>();
				CreateFolderIfMissing("Assets", "Resources");
				CreateFolderIfMissing("Assets/Resources", ResourcesFolder);
				UnityEditor.AssetDatabase.CreateAsset(
					_instance,
					$"Assets/Resources/{ResourcesFolder}/{AssetName}.asset");
				UnityEditor.AssetDatabase.SaveAssets();
#endif
				return _instance;
			}
		}

#if UNITY_EDITOR
		private static void CreateFolderIfMissing(string parent, string folder)
		{
			if (!UnityEditor.AssetDatabase.IsValidFolder($"{parent}/{folder}"))
				UnityEditor.AssetDatabase.CreateFolder(parent, folder);
		}
#endif

		protected override void DoValidate()
		{
			var otherConfigs = Resources.FindObjectsOfTypeAll<TConfig>();
			if (otherConfigs.Length > 1)
			{
				Debug.LogError("There are more than one instance of " + AssetName);
			}
		}
	}
}
