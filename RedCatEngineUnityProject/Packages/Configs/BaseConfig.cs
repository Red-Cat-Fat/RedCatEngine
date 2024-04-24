using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace RedCatEngine.Configs
{
	public class BaseConfig : ScriptableObject
	{
		[ReadOnly]
		public int ConfigId;
		
		public bool Equals(BaseConfig other)
		{
			return ConfigId == other.ConfigId;
		}
		
		private void OnValidate()
		{
#if UNITY_EDITOR
			if (!EditorUtility.IsPersistent(this))
				return;
			ConfigId = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this)).GetHashCode();
#endif
		}

		public override int GetHashCode()
		{
			return ConfigId.GetHashCode();
		}
	}
}