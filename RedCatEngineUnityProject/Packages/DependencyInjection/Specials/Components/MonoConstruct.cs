using System;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using UnityEngine;

namespace RedCatEngine.DependencyInjection.Specials.Components
{
	public abstract class MonoConstruct : MonoBehaviour, IMonoConstruct, IDisposable
	{
		private bool _isInitialize;

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
						true) ==
					null)
					continue;

				isHasInjectMethod = true;
			}

			if (!isHasInjectMethod)
				Debug.LogErrorFormat("Not found [MonoInjectAttribute] in component {0}", currentType);
#endif
		}

		protected virtual void DoValidate()
		{
		}

		private void OnDisable()
		{
			Dispose();
		}

		protected virtual void DoInitialize()
		{
		}

		protected virtual void DoDisposable()
		{
		}

		public void Dispose()
		{
			if (!_isInitialize)
			{
#if UNITY_EDITOR
				Debug.LogWarningFormat("Not initialize element try to dispose in {0}", gameObject.name);
#endif
				return;
			}

			_isInitialize = false;
			DoDisposable();
		}

		public void FinishInitialize()
		{
			DoInitialize();
			_isInitialize = true;
		}
	}
}