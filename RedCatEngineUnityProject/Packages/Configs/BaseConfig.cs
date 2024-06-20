#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RedCatEngine.Configs
{
	public abstract class BaseConfig : ScriptableObject
	{
		[SerializeField]
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
			DoValidate();
#endif
		}

		protected virtual void DoValidate()
		{

		}

		public override bool Equals(object other)
		{
			return other as BaseConfig != null 
				&& other.GetType() == this.GetType() 
				&& Equals((BaseConfig) other);
		}

		private bool Equals(BaseConfig other)
		{
			return _id == other._id;
		}

		public override int GetHashCode() 
			=> _id;
	}
}