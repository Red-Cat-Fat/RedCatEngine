using System;
using System.Collections.Generic;
using System.Linq;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Exceptions;
using RedCatEngine.DependencyInjection.Specials.Providers;

namespace RedCatEngine.DependencyInjection.Containers
{
	public class ApplicationContainer : IApplicationContainer
	{
		private readonly Dictionary<Type, object> _objects = new();
		private readonly CashContainer _cashContainer;
		private readonly ProviderService _providerService;

		public ApplicationContainer()
		{
			_cashContainer = new CashContainer();
			_providerService = new ProviderService();
		}

		public ISingleProvider<TProvideType> RegisterProvider<TProvideType>() where TProvideType : class
			=> _providerService.RegisterProvider<TProvideType>();

		public ISingleProvider<TProvideType> RegisterArrayProvider<TProvideType>() where TProvideType : class
			=> _providerService.RegisterArrayProvider<TProvideType>();

		public TBindType BindAsSingle<TBindType>(TBindType instance)
		{
			var type = typeof(TBindType);
			if (_objects.TryGetValue(type, out var alreadyInstance))
				throw new BindDuplicateWithoutArrayMarkException(typeof(TBindType), alreadyInstance);

			_objects.Add(type, instance);
			return _providerService.BindAsSingle(instance);
		}

		public TBindType BindAsArray<TBindType>(TBindType instance)
		{
			var type = typeof(TBindType);
			if (!_cashContainer.ArrayObjects.ContainsKey(type))
				_cashContainer.ArrayObjects.Add(type, new List<object>());

			_cashContainer.ArrayObjects[type].Add(instance);
			return _providerService.BindAsArray(instance);
		}

		public bool TryGetSingle<T>(out T data)
		{
			var type = typeof(T);
			if (_objects.TryGetValue(type, out var instance))
			{
				data = (T)instance;
				return true;
			}

			if (_cashContainer.TryFindFirstChildByType<T>(_objects, out var parent))
			{
				data = parent;
				return true;
			}

			data = default;
			return false;
		}

		public T GetSingle<T>()
		{
			if (_objects.TryGetValue(typeof(T), out var instance))
				return (T)instance;

			if (_cashContainer.TryFindFirstChildByType<T>(_objects, out var typedInstance))
				return typedInstance;

			throw new NotFoundInstanceException(typeof(T));
		}

		public bool TryGetArray<T>(out IEnumerable<T> data)
		{
			if (!_cashContainer.ArrayObjects.TryGetValue(typeof(T), out var instances))
				return _cashContainer.TryGetAndCachedArrayByOtherKeys(out data)
					|| _cashContainer.TryGetAndCachedArrayByParenFromSingle(_objects, out data);

			data = instances.Select(instance => (T)instance);
			return true;
		}

		public IEnumerable<T> GetArray<T>()
		{
			if (_cashContainer.ArrayObjects.TryGetValue(typeof(T), out var instanceEnumerable))
				return instanceEnumerable.Select(instance => (T)instance);

			if (_cashContainer.TryGetAndCachedArrayByOtherKeys<T>(out var newTypes))
				return newTypes;

			if (_cashContainer.TryGetAndCachedArrayByParenFromSingle<T>(_objects, out var singleVariants))
				return singleVariants;

			throw new NotFoundInstanceException(typeof(T));

		}
	}
}