using System;
using System.Collections.Generic;
using RedCatEngine.DependencyInjection.Containers.Interfaces;

namespace RedCatEngine.DependencyInjection.Specials.Providers
{
	public class ProviderService : IBinderApplicationContainer
	{
		private readonly Dictionary<Type, List<IWait>> _waits = new();

		public IProvider<TProvideType> RegisterProvider<TProvideType>() where TProvideType : class
		{
			var provider = new Provider<TProvideType>();
			var typeKey = typeof(TProvideType);
			if (!_waits.TryGetValue(typeKey, out var waiterList))
			{
				waiterList = new List<IWait>();
				_waits.Add(typeKey, waiterList);
			}

			waiterList.Add(provider);
			return provider;
		}

		public TBindType BindAsSingle<TBindType>(TBindType instance)
		{
			OnNewBind(instance);
			return instance;
		}

		public TBindType BindAsArray<TBindType>(TBindType instance)
		{
			OnNewBind(instance);
			return instance;
		}

		private void OnNewBind<TBindType>(TBindType instance)
		{
			if (!_waits.TryGetValue(typeof(TBindType), out var waiterList))
				return;
			foreach (var waiter in waiterList)
				waiter.Attach(instance);
		}
	}
}