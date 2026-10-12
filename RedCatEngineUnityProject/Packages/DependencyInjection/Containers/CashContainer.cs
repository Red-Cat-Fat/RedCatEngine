using System;
using System.Collections.Generic;
using System.Linq;

namespace RedCatEngine.DependencyInjection.Containers
{
	public class CashContainer
	{
		private readonly ChildTypesCache _childTypesCache;
		internal readonly Dictionary<Type, List<object>> ArrayObjects = new();

		public CashContainer(ChildTypesCache childTypesCache) => _childTypesCache = childTypesCache;

		internal bool TryFindFirstChildByType<T>(Dictionary<Type, object> objects, out T data)
		{
			data = default;
			if (!TryFindFirstChildByType(
				    typeof(T),
				    objects,
				    out var findData))
				return false;

			data = (T)findData;
			return true;
		}

		internal bool TryFindFirstChildByType(
			Type findType,
			Dictionary<Type, object> objects,
			out object data
		)
		{
			var listTypes = GetChildTypes(findType);
			foreach (var type in listTypes)
			{
				if (!objects.TryGetValue(type, out var instance))
					continue;

				var targetInstance = instance;
				if (targetInstance == null)
					continue;

				data = targetInstance;
				return true;
			}

			data = default;
			return false;
		}

		private IEnumerable<Type> GetChildTypes(Type type) => _childTypesCache.GetChildTypes(type);

		internal bool TryGetAndCachedArrayByOtherKeys<T>(out IEnumerable<T> data)
		{
			var childElements = new List<T>();
			foreach (var arraysType in ArrayObjects.Keys)
			{
				foreach (var instance in ArrayObjects[arraysType])
				{
					if (instance is T tInstance)
						childElements.Add(tInstance);
				}
			}

			if (childElements.Any())
			{
				ArrayObjects.Add(typeof(T), childElements.Select(instance => (object)instance).ToList());
				data = childElements;
				return true;
			}

			data = ArraySegment<T>.Empty;
			return false;
		}

		internal bool TryGetAndCachedArrayByParenFromSingle<T>(
			Dictionary<Type, object> objects,
			out IEnumerable<T> data
		)
		{
			var findSomething = TryFindAllChildByType<T>(objects, out var findElements);
			var findEnumerable = Enumerable.ToList<T>(findElements);

			if (findSomething)
				ArrayObjects.Add(typeof(T), findEnumerable.Select(element => (object)element).ToList());

			data = findEnumerable;
			return findSomething;
		}

		private bool TryFindAllChildByType<T>(Dictionary<Type, object> objects, out IEnumerable<T> data)
		{
			var childTypes = GetChildTypes(typeof(T));
			List<T> findTypes = new();

			foreach (var type in childTypes)
			{
				if (!objects.TryGetValue(type, out var instance))
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