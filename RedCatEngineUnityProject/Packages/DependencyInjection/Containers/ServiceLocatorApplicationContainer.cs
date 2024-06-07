using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RedCatEngine.DependencyInjection.Exceptions;

namespace RedCatEngine.DependencyInjection.Containers
{
	public class ServiceLocatorApplicationContainer : IApplicationContainer
	{
		private static readonly Dictionary<Type, Type[]> ChildTypeCache = new();

		private readonly Dictionary<Type, object> _objects = new();
		private readonly Dictionary<Type, List<object>> _arrayObjects = new();

		public void BindAsSingle<T>(T instance)
		{
			var type = typeof(T);
			if (_objects.TryGetValue(type, out var alreadyInstance))
				throw new BindDuplicateWithoutArrayMarkException(typeof(T), alreadyInstance);

			_objects.Add(type, instance);
		}

		public void BindAsArray<T>(T instance)
		{
			var type = typeof(T);
			if (!_arrayObjects.ContainsKey(type))
				_arrayObjects.Add(type, new List<object>());

			_arrayObjects[type].Add(instance);
		}

		public bool TryGetSingle<T>(out T data)
		{
			var type = typeof(T);
			if (_objects.TryGetValue(type, out var instance))
			{
				data = (T)instance;
				return true;
			}

			if (TryFindFirstChildByType<T>(out var parent))
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

			if (TryFindFirstChildByType<T>(out var typedInstance))
				return typedInstance;

			throw new NotFoundInstanceException(typeof(T));
		}

		public bool TryGetArray<T>(out IEnumerable<T> data)
		{
			if (!_arrayObjects.TryGetValue(typeof(T), out var instances))
				return TryGetAndCachedArrayByOtherKeys(out data) || TryGetAndCachedArrayByParenFromSingle(out data);

			data = instances.Select(instance => (T)instance);
			return true;
		}

		public IEnumerable<T> GetArray<T>()
		{
			if (_arrayObjects.TryGetValue(typeof(T), out var instanceEnumerable))
				return instanceEnumerable.Select(instance => (T)instance);

			if (TryGetAndCachedArrayByOtherKeys<T>(out var newTypes))
				return newTypes;

			if (TryGetAndCachedArrayByParenFromSingle<T>(out var singleVariants))
				return singleVariants;
			
			throw new NotFoundInstanceException(typeof(T));

		}

		private bool TryFindFirstChildByType<T>(out T data)
		{
			var listTypes = GetChildTypes(typeof(T));
			foreach (var type in listTypes)
			{
				if (!_objects.TryGetValue(type, out var instance))
					continue;

				var targetInstance = (T)instance;
				if (targetInstance == null)
					continue;

				data = targetInstance;
				return true;
			}

			data = default;
			return false;
		}

		private static IEnumerable<Type> GetChildTypes(Type type)
		{
			if (ChildTypeCache.TryGetValue(type, out var result))
				return result;

			if (type.IsInterface)
				result = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes())
					.Where(p => p != type && type.IsAssignableFrom(p)).ToArray();
			else
				result = Assembly.GetAssembly(type).GetTypes().Where(t => t.IsSubclassOf(type)).ToArray();

			ChildTypeCache[type] = result;

			return result;
		}

		private bool TryGetAndCachedArrayByOtherKeys<T>(out IEnumerable<T> data)
		{
			var childElements = new List<T>();
			foreach (var arraysType in _arrayObjects.Keys)
			{
				foreach (var instance in _arrayObjects[arraysType])
				{
					if (instance is T tInstance)
						childElements.Add(tInstance);
				}
			}

			if (childElements.Any())
			{
				_arrayObjects.Add(typeof(T), childElements.Select(instance => (object)instance).ToList());
				data = childElements;
				return true;
			}

			data = ArraySegment<T>.Empty;
			return false;
		}

		private bool TryGetAndCachedArrayByParenFromSingle<T>(out IEnumerable<T> data)
		{
			var findSomething = TryFindAllChildByType<T>(out var findElements);
			var findEnumerable = findElements.ToList();

			if (findSomething)
				_arrayObjects.Add(typeof(T), findEnumerable.Select(element => (object)element).ToList());

			data = findEnumerable;
			return findSomething;
		}

		private bool TryFindAllChildByType<T>(out IEnumerable<T> data)
		{
			var childTypes = GetChildTypes(typeof(T));
			List<T> findTypes = new();

			foreach (var type in childTypes)
			{
				if (!_objects.TryGetValue(type, out var instance))
					continue;

				var targetInstance = (T)instance;
				if (targetInstance == null)
					continue;

				findTypes.Add(targetInstance);
			}

			data = findTypes;
			return findTypes.Count > 0;
		}
	}
}