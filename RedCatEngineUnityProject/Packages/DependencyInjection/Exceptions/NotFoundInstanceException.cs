using System;

namespace RedCatEngine.DependencyInjection.Exceptions
{
	public class NotFoundInstanceException : Exception
	{
		public readonly Type NotFoundType;
		private const string ErrorMessageFormat = "Not found instances for Type {0}";

		public NotFoundInstanceException(Type notFoundType)
			: base(string.Format(ErrorMessageFormat, notFoundType))
		{
			NotFoundType = notFoundType;
		}
	}
}