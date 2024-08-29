using System;
using System.Collections.Generic;
using RedCatEngine.DependencyInjection.Containers.Interfaces;

namespace RedCatEngine.DependencyInjection.Specials.Providers
{
	public class ProviderService : IBinderApplicationContainer
	{
		private readonly Dictionary<Type, List<IWaiter>> _singleWaits = new();
		private readonly Dictionary<Type, List<IWaiter>> _arrayWaits = new();

		private void AddWaiter(
			Dictionary<Type, List<IWaiter>> waiterCash,
			Type key,
			IWaiter provider
		)
		{
			if (!waiterCash.TryGetValue(key, out var waiterList))
			{
				waiterList = new List<IWaiter>();
				waiterCash.Add(key, waiterList);
			}

			waiterList.Add(provider);
		}

		public ISingleProvider<TProvideType> RegisterProvider<TProvideType>() where TProvideType : class
		{
			var provider = new SingleProvider<TProvideType>();
			AddWaiter(
				_singleWaits,
				typeof(TProvideType),
				provider);
			return provider;
		}

		public object RegisterArrayProvider(Type providerType)
		{
			var arrayProvider = Activator.CreateInstance(typeof(ArrayProvider<>).MakeGenericType(providerType));
			AddWaiter(
				_arrayWaits,
				providerType,
				(IWaiter)arrayProvider);
			return arrayProvider;
		}

		public object RegisterProvider(Type providerType)
		{
			var provider = Activator.CreateInstance(typeof(SingleProvider<>).MakeGenericType(providerType));
			AddWaiter(
				_singleWaits,
				providerType,
				(IWaiter)provider);
			return provider;
		}

		public IArrayProvider<TProvideType> RegisterArrayProvider<TProvideType>() where TProvideType : class
		{
			var provider = new ArrayProvider<TProvideType>();
			AddWaiter(
				_arrayWaits,
				typeof(TProvideType),
				provider);
			return provider;
		}

		public TBindType BindAsSingle<TBindType>(TBindType instance)
		{
			OnNewSingleBind(instance);
			return instance;
		}

		public TBindType BindAsArray<TBindType>(TBindType instance)
		{
			if (!_arrayWaits.TryGetValue(typeof(TBindType), out var waiterList))
				return instance;

			foreach (var waiter in waiterList)
				waiter.Attach(instance);
			return instance;
		}

		private void OnNewSingleBind<TBindType>(TBindType instance)
		{
			if (!_singleWaits.TryGetValue(typeof(TBindType), out var waiterList))
				return;

			foreach (var waiter in waiterList)
				waiter.Attach(instance);
		}

		private void OnNewArrayBind<TBindType>(TBindType instance)
		{
			if (!_arrayWaits.TryGetValue(typeof(TBindType), out var waiterList))
				return;

			foreach (var waiter in waiterList)
				waiter.Attach(instance);
		}
	}
}