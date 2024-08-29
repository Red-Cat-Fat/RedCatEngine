using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Exceptions;
using RedCatEngine.DependencyInjection.Specials.Providers;
using RedCatEngine.DependencyInjection.Utils;

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

		public object RegisterProvider(Type providerType)
			=> _providerService.RegisterProvider(providerType);

		public IArrayProvider<TProvideType> RegisterArrayProvider<TProvideType>() where TProvideType : class
			=> _providerService.RegisterArrayProvider<TProvideType>();

		public object RegisterArrayProvider(Type providerType)
			=> _providerService.RegisterArrayProvider(providerType);

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
			=> (T)GetSingle(typeof(T));

		public bool TryGetArray<T>(out IEnumerable<T> data)
		{
			if (!_cashContainer.ArrayObjects.TryGetValue(typeof(T), out var instances))
				return _cashContainer.TryGetAndCachedArrayByOtherKeys(out data) ||
				       _cashContainer.TryGetAndCachedArrayByParenFromSingle(_objects, out data);

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

		public object Create(Type type, params object[] context)
		{
			var constructors = type.GetConstructors();
			ConstructorInfo emptyParameterConstructor = default;
			foreach (var constructor in constructors)
			{
				if (constructor.GetParameters().Length == 0)
					emptyParameterConstructor = constructor;

				if (Attribute.GetCustomAttribute(
					    constructor,
					    typeof(InjectAttribute),
					    true) ==
				    null)
					continue;

				return InjectContextToConstructor(
					type,
					constructor,
					context);
			}

			if (emptyParameterConstructor != default)
				return Activator.CreateInstance(type);

			throw new NotFountInjectAttributeForConstructorException(type);
		}

		private object InjectContextToConstructor(
			Type type,
			MethodBase constructor,
			object[] context
		)
		{
			var parameters = new List<object>();

			foreach (var parameterInfo in constructor.GetParameters())
			{
				if (typeof(ISingleProvider<>).IsAssignableFromGeneric(
					    parameterInfo.ParameterType,
					    out var expectedSingleWaiterGenericType))
				{
					parameters.Add(_providerService.RegisterProvider(expectedSingleWaiterGenericType[0]));
					continue;
				}

				if (typeof(IArrayProvider<>).IsAssignableFromGeneric(
					    parameterInfo.ParameterType,
					    out var expectedArrayWaiterGenericType))
				{
					parameters.Add(_providerService.RegisterArrayProvider(expectedArrayWaiterGenericType[0]));
					continue;
				}

				var fieldType = parameterInfo.ParameterType;
				parameters.Add(GetSingle(fieldType, context));
			}

			return Activator.CreateInstance(type, parameters);
		}

		private object GetSingle(
			Type type,
			params object[] context
		)
		{
			if (_objects.TryGetValue(type, out var instance))
				return instance;

			if (_cashContainer.TryFindFirstChildByType(
				    type,
				    _objects,
				    out var typedInstance))
				return typedInstance;

			if (TryCreate(
				    type,
				    out instance,
				    context))
				return instance;

			throw new NotFoundInstanceException(type);
		}

		private bool TryCreate(
			Type type,
			out object instance,
			params object[] context
		)
		{
			if (type.IsAbstract || type.IsInterface)
			{
				instance = default;
				return false;
			}

			instance = Create(type, context);
			BindAsSingle(instance);
			return true;
		}

		public TBindType BindType<TBindType, TInstanceType>()
			where TInstanceType : TBindType
			=> BindAsSingle<TBindType>(((ICreator)this).Create<TInstanceType>());
	}
}