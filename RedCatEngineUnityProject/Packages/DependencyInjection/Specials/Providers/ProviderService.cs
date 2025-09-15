using System;
using System.Collections.Generic;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.Binders;
using RedCatEngine.DependencyInjection.Specials.Providers.Waiters;

namespace RedCatEngine.DependencyInjection.Specials.Providers
{
	public class ProviderService : IBinderApplicationContainer
	{
		private readonly Dictionary<Type, List<IWaiter>> _arrayWaits = new();
		private readonly Dictionary<Type, List<IWaiter>> _singleWaits = new();

		public ISingleProvider<TProvideType> RegisterProvider<TProvideType>() where TProvideType : class
		{
			var provider = new SingleProvider<TProvideType>();
			AddWaiter(
				_singleWaits,
				typeof(TProvideType),
				provider
			);
			return provider;
		}

		public object RegisterArrayProvider(Type providerType)
		{
			var arrayProvider = Activator.CreateInstance(typeof(ArrayProvider<>).MakeGenericType(providerType));
			AddWaiter(
				_arrayWaits,
				providerType,
				(IWaiter)arrayProvider
			);
			return arrayProvider;
		}

		public object RegisterProvider(Type providerType)
		{
			var provider = Activator.CreateInstance(typeof(SingleProvider<>).MakeGenericType(providerType));
			AddWaiter(
				_singleWaits,
				providerType,
				(IWaiter)provider
			);
			return provider;
		}

		public IArrayProvider<TProvideType> RegisterArrayProvider<TProvideType>() where TProvideType : class
		{
			var provider = new ArrayProvider<TProvideType>();
			AddWaiter(
				_arrayWaits,
				typeof(TProvideType),
				provider
			);
			return provider;
		}

		public TBindType BindAsSingle<TBindType>(TBindType instance)
		{
			if (instance is IWaiter instanceWaiter)
			{
				if (instanceWaiter is ISingleWaiter singleWaiter)
					foreach (var expectedType in singleWaiter.ExpectedTypes)
						AddWaiter(
							_singleWaits,
							expectedType,
							singleWaiter
						);
				if (instanceWaiter is IArrayWaiter arrayWaiter)
					foreach (var expectedType in arrayWaiter.ExpectedTypes)
						AddWaiter(
							_arrayWaits,
							expectedType,
							arrayWaiter
						);
			}

			if (!_singleWaits.TryGetValue(typeof(TBindType), out var waiterList))
				return instance;

			foreach (var waiter in waiterList)
				waiter.Attach(instance);
			return instance;
		}

		public TBindType ReBindAsSingle<TBindType>(TBindType newInstance)
		{
			return BindAsArray(newInstance);
		}

		public TBindType BindAsArray<TBindType>(TBindType instance)
		{
			if (!_arrayWaits.TryGetValue(typeof(TBindType), out var waiterList))
				return instance;

			foreach (var waiter in waiterList)
				waiter.Attach(instance);
			return instance;
		}

		private void AddWaiter(
			IDictionary<Type, List<IWaiter>> waitersCashList,
			Type key,
			IWaiter provider
		)
		{
			if (!waitersCashList.TryGetValue(key, out var waiterList))
			{
				waiterList = new List<IWaiter>();
				waitersCashList.Add(key, waiterList);
			}

			waiterList.Add(provider);
		}
	}
}