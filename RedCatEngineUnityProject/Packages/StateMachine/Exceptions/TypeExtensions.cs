using System;
using System.Collections.Concurrent;

namespace RedCatEngine.CommonServices.Extensions
{
	public static class TypeExtensions
	{
		private static readonly ConcurrentDictionary<Type, string> ShortNameCache = new();

		public static string ShortName(this Type type)
		{
			if (type == null)
				return "null";

			return ShortNameCache.GetOrAdd(
				type,
				t =>
				{
					var fullName = t.FullName ?? t.Name;
					var lastDotIndex = fullName.LastIndexOf('.');
					return lastDotIndex >= 0
						? fullName[(lastDotIndex + 1)..]
						: fullName;
				}
			);
		}
	}
}