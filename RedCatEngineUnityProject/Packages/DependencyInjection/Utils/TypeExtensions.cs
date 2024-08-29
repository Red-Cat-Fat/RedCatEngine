using System;

namespace RedCatEngine.DependencyInjection.Utils
{
	public static class TypeExtensions
	{
		public static bool IsAssignableFromGeneric(
			this Type genericType,
			Type differentType,
			out Type[] expectedGeneric
		)
		{
			if (!genericType.IsGenericTypeDefinition)
			{
				expectedGeneric = genericType.IsGenericType
					? genericType.GetGenericArguments()
					: new[] { genericType };
				return genericType.IsAssignableFrom(differentType);
			}

			do
			{
				var parentInterfaces = differentType.GetInterfaces();
				foreach (var parentInterface in parentInterfaces)
				{
					if (!parentInterface.IsGenericType)
						continue;

					if (parentInterface.GetGenericTypeDefinition() != genericType)
						continue;

					expectedGeneric = parentInterface.GetGenericArguments();
					return true;
				}

				if (differentType.IsGenericType && differentType.GetGenericTypeDefinition() == genericType)
				{
					expectedGeneric = differentType.GetGenericArguments();
					return true;
				}

				differentType = differentType.BaseType;
			} while (differentType != null);

			expectedGeneric = null;
			return false;
		}
	}
}