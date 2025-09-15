using System.Collections.Generic;
using System.Linq;

namespace RedCatEngine.CommonServices.Extensions
{
	public static class ArrayExtensions
	{
		public static object[] WrapInArrayAsSingle(this object singleObject)
			=> new[] { singleObject };

		public static object[] Attach<T>(this object[] array, T value)
			=> array.Union(new object[] { value }).ToArray();

		public static object[] Attach(this object[] array, params object[] values)
			=> array.Union(values).ToArray();

		public static object[] AttachAsSingle<T>(this object[] array, T[] value)
			=> array.Union(new[] { value }).ToArray();

		public static object[] AttachAsSingle<T>(this object[] array, List<T> value)
			=> array.Union(new[] { value }).ToArray();
	}
}