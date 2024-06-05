using System;
using System.Collections.Generic;

namespace RedCatEngine.DependencyInjection
{
	public class ServiceLocatorApplicationContainer : IApplicationContainer
	{
		private readonly Dictionary<Type, object> _objects = new();

		public void Bind<T>(T instance) 
			=> _objects.Add(typeof(T), instance);

		public bool TryGet<T>(out T data)
		{
			if (!_objects.TryGetValue(typeof(T), out var instance))
			{
				data = default;
				return false;
			}

			data = (T)instance;
			return true;
		}
		
		public T Get<T>()
		{
			if (!_objects.TryGetValue(typeof(T), out var instance))
				throw new Exception($"Not found {typeof(T)}");

			return (T)instance;
		}
	}
}