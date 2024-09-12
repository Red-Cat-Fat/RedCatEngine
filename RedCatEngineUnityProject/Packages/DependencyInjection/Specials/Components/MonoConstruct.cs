using System;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using UnityEngine;

namespace RedCatEngine.DependencyInjection.Specials.Components
{
	public abstract class MonoConstruct : MonoBehaviour
	{
		private void OnValidate()
		{
#if UNITY_EDITOR
			DoValidate();

			var isHasInjectMethod = false;
			var currentType = GetType();
			var methods = currentType.GetMethods();
			foreach (var method in methods)
			{
				if (Attribute.GetCustomAttribute(
					    method,
					    typeof(MonoInjectAttribute),
					    true) !=
				    null)
				{
					isHasInjectMethod = true;
				}
			}

			if (!isHasInjectMethod)
				Debug.LogErrorFormat("Not found [MonoInjectAttribute] in component {0}", currentType);
#endif
		}

		protected virtual void DoValidate() { }
	}
}