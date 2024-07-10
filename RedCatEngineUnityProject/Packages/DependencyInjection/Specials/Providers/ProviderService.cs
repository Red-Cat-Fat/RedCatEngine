using System;
using System.Collections.Generic;
using RedCatEngine.DependencyInjection.Containers.Interfaces;

namespace RedCatEngine.DependencyInjection.Specials.Providers
{
	public class ProviderService : IBinderApplicationContainer
	{
		private readonly Dictionary<Type, List<IWaiter>> _waits = new();
		private readonly Dictionary<Type, List<IWaiter>> _arrayWaits = new();

		public ISingleProvider<TProvideType> RegisterProvider<TProvideType>() where TProvideType : class
		{
			var provider = new SingleProvider<TProvideType>();
			var typeKey = typeof(TProvideType);
			if (!_waits.TryGetValue(typeKey, out var waiterList))
			{
				waiterList = new List<IWaiter>();
				_waits.Add(typeKey, waiterList);
			}

			waiterList.Add(provider);
			return provider;
		}

		public ISingleProvider<TProvideType> RegisterArrayProvider<TProvideType>() where TProvideType : class
		{
			var provider = new SingleProvider<TProvideType>();
			var typeKey = typeof(TProvideType);
			if (!_arrayWaits.TryGetValue(typeKey, out var waiterList))
			{
				waiterList = new List<IWaiter>();
				_arrayWaits.Add(typeKey, waiterList);
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
			if (_arrayWaits.TryGetValue(typeof(TBindType), out var waiterList))
				foreach (var waiter in waiterList)
					waiter.Attach(instance);
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