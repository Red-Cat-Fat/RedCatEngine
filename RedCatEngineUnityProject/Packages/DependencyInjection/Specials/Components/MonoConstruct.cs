using System;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using UnityEngine;

namespace RedCatEngine.DependencyInjection.Specials.Components
{
	public abstract class MonoConstruct : MonoBehaviour, IMonoConstruct, IDisposable
	{
		private bool _isDisposed;
		private bool _isInitialize;

		private void OnDisable()
		{
			if(_isInitialize)
				Dispose();
		}

		private void OnValidate()
		{
#if UNITY_EDITOR
			DoValidate();

			var isHasInjectMethod = false;
			var currentType = GetType();
			var methods = currentType.GetMethods();
			foreach(var method in methods)
			{
				if(Attribute.GetCustomAttribute(
						   method,
						   typeof(MonoInjectAttribute),
						   true) ==
				   null)
					continue;

				isHasInjectMethod = true;
			}

			if(!isHasInjectMethod)
				Debug.LogErrorFormat("Not found [MonoInjectAttribute] in component {0}", currentType);
#endif
		}

		public void Dispose()
		{
			if(!_isInitialize)
			{
#if UNITY_EDITOR
				if(!_isDisposed)
					Debug.LogWarningFormat("Not initialize element try to dispose in {0}", gameObject.name);
#endif
				return;
			}

			_isInitialize = false;
			_isDisposed = true;
			DoDispose();
		}

		public bool IsInitialized => _isInitialize;

		public void FinishInitialize()
		{
			DoInitialize();
			_isInitialize = true;
			_isDisposed = false;
		}

		protected virtual void DoValidate()
		{
		}

		protected virtual void DoInitialize()
		{
		}

		protected virtual void DoDispose()
		{
		}
	}
}
