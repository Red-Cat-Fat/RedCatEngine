using System;

namespace RedCatEngine.DependencyInjection.Exceptions
{
	public class NotFoundInstanceOrCreateException : Exception
	{
		public readonly Type NotFoundType;
		private const string ErrorMessageFormat = "Not found instances or create for Type {0}";

		public NotFoundInstanceOrCreateException(Type notFoundType)
			: base(string.Format(ErrorMessageFormat, notFoundType))
		{
			NotFoundType = notFoundType;
		}
	}
}