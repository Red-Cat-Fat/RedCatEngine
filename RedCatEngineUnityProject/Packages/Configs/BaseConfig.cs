using UnityEditor;
using UnityEngine;

namespace RedCatEngine.Configs
{
	public abstract class BaseConfig : ScriptableObject
	{
		private int _id;

		public int ID
			=> _id;

		private void OnValidate()
		{
#if UNITY_EDITOR
			if (!EditorUtility.IsPersistent(this))
				return;
			if (_id == 0)
				_id = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this)).GetHashCode();
#endif
			DoValidate();
		}

		protected virtual void DoValidate()
		{

		}

		public bool Equals(BaseConfig other)
		{
			return _id == other._id;
		}

		public override int GetHashCode() 
			=> _id;
	}
}