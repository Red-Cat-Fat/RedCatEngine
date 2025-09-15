using System;
using System.Collections.Generic;
using System.Reflection;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Exceptions;
using RedCatEngine.DependencyInjection.Specials.Providers;
using RedCatEngine.DependencyInjection.Utils;

namespace RedCatEngine.DependencyInjection.Specials
{
	public class Injector
	{
		private readonly IGetterApplicationContainer _getter;
		private readonly ProviderService _providerService;

		public Injector(IGetterApplicationContainer getter, ProviderService providerService)
		{
			_getter = getter;
			_providerService = providerService;
		}

		public object InjectContextToConstructor(
			Type type,
			MethodBase constructor,
			object[] context
		)
		{
			var parameters = GetParametersForMethod(constructor, context);
			return Activator.CreateInstance(type, parameters);
		}

		public void InjectContextToMethodsWithAttribute<TAttribute>(object objectToInject, params object[] context)
			where TAttribute : Attribute
		{
			var type = objectToInject.GetType();
			var methods = type.GetMethods();
			var findConstructor = false;
			foreach (var method in methods)
			{
				if (Attribute.GetCustomAttribute(
						method,
						typeof(TAttribute),
						true)
					== null)
					continue;

				var parameters = GetParametersForMethod(method, context);
				method.Invoke(objectToInject, parameters);
				findConstructor = true;
			}
			if (findConstructor)
				return;
			throw new NotFountInjectAttributeForConstructorException<TAttribute>(type);
		}

		private object[] GetParametersForMethod(MethodBase method, object[] context)
		{
			var parameters = new List<object>();

			foreach (var parameterInfo in method.GetParameters())
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

				parameters.Add(_getter.GetSingle(parameterInfo.ParameterType, context));
			}
			return parameters.ToArray();
		}
	}
}