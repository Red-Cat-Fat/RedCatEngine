using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Exceptions;
using RedCatEngine.DependencyInjection.Specials.Providers;
using RedCatEngine.DependencyInjection.Utils;

namespace RedCatEngine.DependencyInjection.Containers
{
	public class ApplicationContainer : IApplicationContainer
	{
		private readonly Dictionary<Type, object> _objects = new();
		private readonly CashContainer _cashContainer;
		protected readonly ProviderService _providerService;

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

			throw new NotFoundInstanceOrCreateException(typeof(T));
		}

		public T Create<T>(params object[] context)
			=> (T)Create(typeof(T), context);

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

				parameters.Add(GetSingle(parameterInfo.ParameterType, context));
			}

			return Activator.CreateInstance(type, parameters.ToArray());
		}

		public T GetSingle<T>(params object[] context)
			=> (T)GetSingle(typeof(T), context);

		public object GetSingle(
			Type type,
			params object[] context
		)
		{
			foreach (var contextParameter in context)
			{
				if (type.IsInstanceOfType(contextParameter))
					return contextParameter;
				if (contextParameter is not object[] arrayObjects)
					continue;

				foreach (var contextObject in arrayObjects)
				{
					if (type.IsInstanceOfType(contextObject))
						return contextObject;
				}
			}

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

			throw new NotFoundInstanceOrCreateException(type);
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
			BindAsSingle(type, instance);
			return true;
		}

		public TInstanceBindType BindDummy<TInstanceBindType, TDummyType>(params object[] context)
			where TDummyType : TInstanceBindType
		{
			if (!TryGetSingle<TInstanceBindType>(out var instance))
				instance = BindType<TInstanceBindType, TDummyType>();
			return instance;
		}

		public TBindType BindType<TBindType, TInstanceType>(params object[] context)
			where TInstanceType : TBindType
			=> BindAsSingle<TBindType>(Create<TInstanceType>(context));

		public TBindArrayType BindArrayType<TBindArrayType, TInstanceType>(params object[] context)
			where TInstanceType : TBindArrayType
			=> BindAsArray(typeof(TBindArrayType), Create<TInstanceType>(context));

		public TInstanceBindType BindType<TInstanceBindType>(params object[] context)
			=> BindAsSingle(Create<TInstanceBindType>(context));

		public object BindType(Type type, params object[] context)
			=> BindAsSingle(type, Create(type, context));

		public TInstanceBindType BindArrayType<TInstanceBindType>(params object[] context)
			=> BindAsArray(Create<TInstanceBindType>(context));

		public object BindArrayType(Type type, params object[] context)
			=> BindAsArray(type, Create(type, context));

		public TBindType BindAsSingle<TBindType>(TBindType instance)
			=> BindAsSingle(typeof(TBindType), instance);

		protected TBindType BindAsSingle<TBindType>(Type typeKey, TBindType instance)
		{
			if (_objects.TryGetValue(typeKey, out var alreadyInstance))
				throw new BindDuplicateWithoutArrayMarkException(typeof(TBindType), alreadyInstance);

			_objects.Add(typeKey, instance);
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

		protected TBindType BindAsArray<TBindType>(Type typeKey, TBindType instance)
		{
			if (!_cashContainer.ArrayObjects.ContainsKey(typeKey))
				_cashContainer.ArrayObjects.Add(typeKey, new List<object>());

			_cashContainer.ArrayObjects[typeKey].Add(instance);
			return _providerService.BindAsArray(instance);
		}
	}
}