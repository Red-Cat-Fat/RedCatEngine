using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Exceptions;
using RedCatEngine.DependencyInjection.Specials;
using RedCatEngine.DependencyInjection.Specials.Providers;

namespace RedCatEngine.DependencyInjection.Containers
{
	public class ApplicationContainer : IApplicationContainer
	{
		private readonly IApplicationContainer _parent;
		private readonly CashContainer _cashContainer;
		private readonly Dictionary<Type, object> _objects = new();
		private readonly ProviderService _providerService;
		private readonly List<IApplicationContainer> _chilContainers = new();

		private bool _isDisposed;

		public ApplicationContainer()
		{
			_cashContainer = new CashContainer();
			_providerService = new ProviderService();
			Injector = new Injector(this, _providerService);
		}

		protected ApplicationContainer(IApplicationContainer parent)
		{
			_parent = parent;
			_cashContainer = new CashContainer();
			_providerService = new ProviderService();
			Injector = new Injector(this, _providerService);
		}

		public virtual IApplicationContainer CreateChildContainer()
		{
			var childContainer = new ApplicationContainer(this);
			_chilContainers.Add(childContainer);
			return childContainer;
		}

		public Injector Injector { get; }

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
			if (TryGetSingle(typeof(T), out var obj)
				&& obj is T typedObj)
			{
				data = typedObj;
				return true;
			}

			data = default;
			return false;
		}


		public bool TryGetSingle(Type type, out object data)
		{
			if (_objects.TryGetValue(type, out var instance))
			{
				data = instance;
				return true;
			}

			if (_cashContainer.TryFindFirstChildByType(
					type,
					_objects,
					out var parent) &&
				parent != null &&
				type.IsAssignableFrom(parent.GetType()))
			{
				data = parent;
				return true;
			}

			if (_parent != null)
				return _parent.TryGetSingle(type, out data);

			data = null;
			return false;
		}

		public bool TryGetArray<T>(out IEnumerable<T> data)
		{
			if (!_cashContainer.ArrayObjects.TryGetValue(typeof(T), out var instances))
				return _cashContainer.TryGetAndCachedArrayByOtherKeys(out data) ||
					_cashContainer.TryGetAndCachedArrayByParenFromSingle(_objects, out data);

			data = instances.OfType<T>().ToList();

			if (!data.Any() && _parent != null)
				return _parent.TryGetArray(out data);

			return data.Any();
		}


		public IEnumerable<T> GetArray<T>()
		{
			if (_cashContainer.ArrayObjects.TryGetValue(typeof(T), out var instanceEnumerable))
				return instanceEnumerable.Select(instance => (T)instance);

			if (_cashContainer.TryGetAndCachedArrayByOtherKeys<T>(out var newTypes))
				return newTypes;

			if (_cashContainer.TryGetAndCachedArrayByParenFromSingle<T>(_objects, out var singleVariants))
				return singleVariants;

			if (_parent != null)
				return _parent.GetArray<T>();

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

				return Injector.InjectContextToConstructor(
					type,
					constructor,
					context);
			}

			if (emptyParameterConstructor != default)
				return Activator.CreateInstance(type);

			throw new NotFountInjectAttributeForConstructorException<InjectAttribute>(type);
		}

		public T GetSingle<T>(params object[] context)
		{
			if (GetSingle(typeof(T), context) is T result)
				return result;

			throw new InvalidCastException($"Object of type {typeof(T)} could not be cast.");
		}


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

			if (_parent != null && _parent.TryGetSingle(type, out var parentInstance))
				return parentInstance;

			if (TryCreate(
				type,
				out var createdInstance,
				context) && type.IsInstanceOfType(createdInstance))
				return createdInstance;

			throw new NotFoundInstanceOrCreateException(type);
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

		public TBindType ReBindAsSingle<TBindType>(TBindType newInstance)
		{
			var typeKey = typeof(TBindType);
			if (!_objects.TryGetValue(typeKey, out _))
				return BindAsSingle(newInstance);
			_objects[typeKey] = newInstance;
			return _providerService.ReBindAsSingle(newInstance);
		}

		public TBindType BindAsArray<TBindType>(TBindType instance)
		{
			var type = typeof(TBindType);
			if (!_cashContainer.ArrayObjects.ContainsKey(type))
				_cashContainer.ArrayObjects.Add(type, new List<object>());

			_cashContainer.ArrayObjects[type].Add(instance);
			return _providerService.BindAsArray(instance);
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

		protected TBindType BindAsSingle<TBindType>(Type typeKey, TBindType instance)
		{
			if (_objects.TryGetValue(typeKey, out var alreadyInstance))
				throw new BindDuplicateWithoutArrayMarkException(typeof(TBindType), alreadyInstance);

			_objects.Add(typeKey, instance);
			return _providerService.BindAsSingle(instance);
		}

		protected TBindType BindAsArray<TBindType>(Type typeKey, TBindType instance)
		{
			if (!_cashContainer.ArrayObjects.ContainsKey(typeKey))
				_cashContainer.ArrayObjects.Add(typeKey, new List<object>());

			_cashContainer.ArrayObjects[typeKey].Add(instance);
			return _providerService.BindAsArray(instance);
		}

		public void Dispose()
		{
			if (_isDisposed)
				return;

			for (var index = 0; index < _chilContainers.Count; index++)
				_chilContainers[index].Dispose();

			var disposablesSingleObjects = _objects.Values
				.OfType<IDisposable>()
				.ToList();
			foreach (var disposable in disposablesSingleObjects)
			{
				disposable.Dispose();
			}

			if (!_cashContainer.TryGetAndCachedArrayByOtherKeys<IDisposable>(
				out var disposablesArrays))
				return;
			foreach (var disposable in disposablesArrays)
				disposable.Dispose();

			_isDisposed = true;
		}
	}
}