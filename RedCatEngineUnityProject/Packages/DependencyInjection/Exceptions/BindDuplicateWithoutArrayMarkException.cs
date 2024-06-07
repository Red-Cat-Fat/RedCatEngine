using System;

namespace RedCatEngine.DependencyInjection.Exceptions
{
	public class BindDuplicateWithoutArrayMarkException : Exception
	{
		public readonly Type DuplicateType;
		public readonly object Instance;
		private const string ErrorMessageFormat =
			"Type {0} already bind as single object (contain instance: {1}). If you try bind as array elemrnt need bind as array.";

		public BindDuplicateWithoutArrayMarkException(Type duplicateType, object instance)
			: base(string.Format(ErrorMessageFormat, duplicateType, instance))
		{
			DuplicateType = duplicateType;
			Instance = instance;
		}
	}
}