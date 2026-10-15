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
			BindAsSingle(typeof(TBindType), instance);
			return instance;
		}

		/// <summary>
		/// Notifies the single waiters of <paramref name="bindType"/>. The key is passed explicitly, because the
		/// container binds instances under a type that can differ from the static type of the instance
		/// (BindType&lt;IService, Service&gt;, BindType(Type), auto-create).
		/// </summary>
		public void BindAsSingle(Type bindType, object instance)
		{
			RegisterInstanceAsWaiter(instance);
			NotifyWaiters(_singleWaits, bindType, instance);
		}

		public TBindType ReBindAsSingle<TBindType>(TBindType newInstance)
		{
			ReBindAsSingle(typeof(TBindType), newInstance);
			return newInstance;
		}

		public void ReBindAsSingle(Type bindType, object newInstance)
			=> NotifyWaiters(_singleWaits, bindType, newInstance);

		public TBindType BindAsArray<TBindType>(TBindType instance)
		{
			BindAsArray(typeof(TBindType), instance);
			return instance;
		}

		/// <summary>
		/// Notifies the array waiters of <paramref name="bindType"/>; see <see cref="BindAsSingle(Type, object)"/>.
		/// </summary>
		public void BindAsArray(Type bindType, object instance)
			=> NotifyWaiters(_arrayWaits, bindType, instance);

		private void RegisterInstanceAsWaiter(object instance)
		{
			if (instance is not IWaiter)
				return;

			if (instance is ISingleWaiter singleWaiter)
				foreach (var expectedType in singleWaiter.ExpectedTypes)
					AddWaiter(
						_singleWaits,
						expectedType,
						singleWaiter
					);
			if (instance is IArrayWaiter arrayWaiter)
				foreach (var expectedType in arrayWaiter.ExpectedTypes)
					AddWaiter(
						_arrayWaits,
						expectedType,
						arrayWaiter
					);
		}

		private static void NotifyWaiters(IDictionary<Type, List<IWaiter>> waiters, Type bindType, object instance)
		{
			if (!waiters.TryGetValue(bindType, out var waiterList))
				return;

			foreach (var waiter in waiterList)
				waiter.Attach(instance);
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