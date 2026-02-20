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
		public const string RequesterTypeContextKey = "__requester_type_context__";

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
			var parameterContext = new object[context.Length + 1];
			Array.Copy(context, parameterContext, context.Length);
			parameterContext[context.Length] = new KeyValuePair<string, Type>(
				RequesterTypeContextKey,
				method.DeclaringType);

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

				parameters.Add(_getter.GetSingle(parameterInfo.ParameterType, parameterContext));
			}
			return parameters.ToArray();
		}
	}
}
