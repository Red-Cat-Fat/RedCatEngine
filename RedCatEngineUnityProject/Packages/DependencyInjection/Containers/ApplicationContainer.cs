using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Exceptions;
using RedCatEngine.DependencyInjection.Specials;
using RedCatEngine.DependencyInjection.Specials.CreatedRule;
using RedCatEngine.DependencyInjection.Specials.Diagnostics;
using RedCatEngine.DependencyInjection.Specials.Providers;

namespace RedCatEngine.DependencyInjection.Containers
{
	public class ApplicationContainer : IApplicationContainer
	{
		private readonly CashContainer _cashContainer;
		private readonly ChildTypesCache _childTypesCache;
		protected readonly List<IApplicationContainer> _childContainers = new();

		private readonly IContainerDiagnostics _diagnostics;
		private readonly List<IDisposable> _disposable = new();
		private readonly Dictionary<Type, object> _objects = new();

		private readonly IApplicationContainer _parent;
		private readonly ProviderService _providerService;
		private bool _isDisposed;

		public ApplicationContainer(
				ContainerModifyRule containerModifyRules = null,
				IContainerDiagnostics diagnostics = null
		)
		{
			_diagnostics = diagnostics ?? NullContainerDiagnostics.Instance;
			_childTypesCache = new ChildTypesCache(_diagnostics);
			_cashContainer = new CashContainer(_childTypesCache);
			_providerService = new ProviderService();
			Injector = new Injector(this, _providerService);
			Name = "Main";
			if(containerModifyRules != null)
				containerModifyRules.Build(this);
		}

		protected ApplicationContainer(IApplicationContainer parent, string name, ContainerModifyRule containerModifyRules)
		{
			Name = $"{parent.Name}->{name}";
			_parent = parent;
			_diagnostics = parent.Diagnostics;
			_childTypesCache = ResolveChildTypesCache(parent);
			_cashContainer = new CashContainer(_childTypesCache);
			_providerService = new ProviderService();
			Injector = new Injector(this, _providerService);

			if(containerModifyRules != null)
				containerModifyRules.Build(this);
		}

		/// <summary>
		/// Кэш наследников общий на всё дерево контейнеров: дочерние контейнеры создаются на каждого
		/// игрока и каждого NPC, и собственный кэш означал бы полный скан сборок на каждый спавн.
		/// </summary>
		private ChildTypesCache ResolveChildTypesCache(IApplicationContainer parent)
			=> parent is ApplicationContainer typedParent
					? typedParent._childTypesCache
					: new ChildTypesCache(_diagnostics);

		public string Name { get; }

		public IReadOnlyDictionary<Type, object> Objects => _objects;
		public IReadOnlyList<IApplicationContainer> ChildContainers => _childContainers;
		public bool IsDisposed => _isDisposed;

		public IContainerDiagnostics Diagnostics => _diagnostics;

		public virtual IApplicationContainer CreateChildContainer(string name, ContainerModifyRule containerModifyRules)
		{
			Diagnostics.Trace("[DI] Create child container {0}", name);
			var childContainer = new ApplicationContainer(this, name, containerModifyRules);
			_childContainers.Add(childContainer);

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
			if(TryGetSingle(typeof(T), out var obj)
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
			if(_objects.TryGetValue(type, out var instance))
			{
				data = instance;
				return true;
			}

			if(_cashContainer.TryFindFirstChildByType(
					   type,
					   _objects,
					   out var parentInstance
			   )
			   && parentInstance != null
			   && type.IsAssignableFrom(parentInstance.GetType()))
			{
				data = parentInstance;
				return true;
			}

			if(_parent != null)
				return _parent.TryGetSingle(type, out data);

			data = null;
			return false;
		}

		public bool TryGetArray<T>(out IEnumerable<T> data)
		{
			if(!_cashContainer.ArrayObjects.TryGetValue(typeof(T), out var instances))
				return _cashContainer.TryGetAndCachedArrayByOtherKeys(out data)
				       || _cashContainer.TryGetAndCachedArrayByParenFromSingle(_objects, out data);

			data = instances.OfType<T>().ToList();

			if(!data.Any()
			   && _parent != null)
				return _parent.TryGetArray(out data);

			return data.Any();
		}


		public IEnumerable<T> GetArray<T>()
		{
			if(_cashContainer.ArrayObjects.TryGetValue(typeof(T), out var instanceEnumerable))
				return instanceEnumerable.Select(instance => (T)instance);

			if(_cashContainer.TryGetAndCachedArrayByOtherKeys<T>(out var newTypes))
				return newTypes;

			if(_cashContainer.TryGetAndCachedArrayByParenFromSingle<T>(_objects, out var singleVariants))
				return singleVariants;

			if(_parent != null)
				return _parent.GetArray<T>();

			return Enumerable.Empty<T>();
		}

		public T Create<T>(params object[] context)
			=> (T)Create(typeof(T), context);

		public object Create(Type type, params object[] context)
		{
			var constructors = type.GetConstructors();
			ConstructorInfo emptyParameterConstructor = default;
			foreach(var constructor in constructors)
			{
				if(constructor.GetParameters().Length == 0)
					emptyParameterConstructor = constructor;

				if(Attribute.GetCustomAttribute(
						   constructor,
						   typeof(InjectAttribute),
						   true
				   )
				   == null)
					continue;
				var result = Injector.InjectContextToConstructor(
						type,
						constructor,
						context
				);
				return TryRegisterAsDisposable(result);
			}

			if(emptyParameterConstructor != default)
				return Activator.CreateInstance(type);

			throw new NotFountInjectAttributeForConstructorException<InjectAttribute>(type);
		}

		public T GetSingle<T>(params object[] context)
		{
			if(GetSingle(typeof(T), context) is T result)
				return result;

			throw new InvalidCastException($"Object of type {typeof(T)} could not be cast.");
		}


		public object GetSingle(
				Type type,
				params object[] context
		)
		{
			// ищем в контексте
			foreach(var contextParameter in context)
			{
				if(type.IsInstanceOfType(contextParameter))
					return contextParameter;

				if(contextParameter is not object[] arrayObjects)
					continue;

				foreach(var contextObject in arrayObjects)
				{
					if(type.IsInstanceOfType(contextObject))
						return contextObject;
				}
			}

			// ищем в забинденных обжах
			if(_objects.TryGetValue(type, out var instance))
				return instance;

			// ищем в дочерних типах
			if(_cashContainer.TryFindFirstChildByType(
					   type,
					   _objects,
					   out var typedInstance
			   ))
				return typedInstance;

			if(_parent != null
			   && _parent.TryGetSingle(type, out var parentInstance))
				return parentInstance;

			if(TryCreate(
					   type,
					   out var createdInstance,
					   context
			   )
			   && type.IsInstanceOfType(createdInstance))
				return createdInstance;

			throw new NotFoundInstanceOrCreateException(type);
		}

		public TInstanceBindType BindDummy<TInstanceBindType, TDummyType>(params object[] context)
				where TDummyType : TInstanceBindType
		{
			if(!TryGetSingle<TInstanceBindType>(out var instance))
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
			if(!_objects.TryGetValue(typeKey, out _))
				return BindAsSingle(newInstance);
			_objects[typeKey] = newInstance;
			_providerService.ReBindAsSingle(typeKey, newInstance);
			return newInstance;
		}

		public TBindType BindAsArray<TBindType>(TBindType instance)
		{
			var type = typeof(TBindType);
			if(!_cashContainer.ArrayObjects.ContainsKey(type))
				_cashContainer.ArrayObjects.Add(type, new List<object>());

			_cashContainer.ArrayObjects[type].Add(instance);
			_providerService.BindAsArray(type, instance);
			return instance;
		}

		public void Dispose()
		{
			if(_isDisposed)
				return;
			_isDisposed = true;

			DetachFromParent();
			DisposeChildContainers();

			foreach(var disposable in _disposable)
				disposable.Dispose();

			if(!_cashContainer.TryGetAndCachedArrayByOtherKeys<IDisposable>(
					   out var disposablesArrays
			   ))
				return;

			foreach(var disposable in disposablesArrays)
				disposable.Dispose();
		}

		/// <summary>
		/// Снимает контейнер с учёта у родителя: без этого выгруженный ребёнок остаётся под ссылкой
		/// родителя до конца его жизни, а вместе с ним — все забинженные объекты и <see cref="IDisposable"/>.
		/// Критично для контейнеров рантайм-объектов (игроки, NPC), которые создаются и выгружаются
		/// многократно за сессию.
		/// </summary>
		private void DetachFromParent()
		{
			if(_parent is ApplicationContainer parentContainer)
				parentContainer._childContainers.Remove(this);
		}

		/// <summary>
		/// Дети выгружаются по снимку: <see cref="DetachFromParent"/> ребёнка правит
		/// <see cref="_childContainers"/> этого контейнера, поэтому обход самого списка терял бы элементы.
		/// </summary>
		private void DisposeChildContainers()
		{
			var children = _childContainers.ToArray();
			_childContainers.Clear();

			foreach(var child in children)
				child.Dispose();
		}

		protected object TryRegisterAsDisposable(object result)
		{
			if(result is IDisposable disposable
			   && !_disposable.Contains(disposable))
				_disposable.Add(disposable);
			return result;
		}

		/// <summary>
		/// Автосоздание при неудачном резолве. Типы, унаследованные от <see cref="UnityEngine.Object"/>,
		/// не создаются: Activator даёт для них фейковый экземпляр (MonoBehaviour вне сцены,
		/// ScriptableObject мимо CreateInstance), который молча биндился бы как single и ломал
		/// последующий легитимный бинд. Для них вызывающий <c>GetSingle</c> бросает
		/// <see cref="NotFoundInstanceOrCreateException"/> — такие экземпляры биндятся только явно.
		/// </summary>
		private bool TryCreate(
				Type type,
				out object instance,
				params object[] context
		)
		{
			if(type.IsAbstract
			   || type.IsInterface
			   || typeof(UnityEngine.Object).IsAssignableFrom(type))
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
			if(_objects.TryGetValue(typeKey, out var alreadyInstance))
				throw new BindDuplicateWithoutArrayMarkException(typeof(TBindType), alreadyInstance);

			_objects.Add(typeKey, instance);
			_providerService.BindAsSingle(typeKey, instance);
			return instance;
		}

		protected TBindType BindAsArray<TBindType>(Type typeKey, TBindType instance)
		{
			if(!_cashContainer.ArrayObjects.ContainsKey(typeKey))
				_cashContainer.ArrayObjects.Add(typeKey, new List<object>());

			_cashContainer.ArrayObjects[typeKey].Add(instance);
			_providerService.BindAsArray(typeKey, instance);
			return instance;
		}
	}
}
