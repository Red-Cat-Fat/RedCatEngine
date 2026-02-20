using System;

namespace RedCatEngine.DependencyInjection.Exceptions
{
	public class NotFoundInstanceOrCreateException : Exception
	{
		public readonly Type NotFoundType;
		public readonly Type RequesterType;
		private const string ErrorMessageFormat = "Not found instances or create for Type {0}";
		private const string ErrorMessageWithRequesterFormat =
			"Not found instances or create for Type {0}. Requested by Type {1}";

		public NotFoundInstanceOrCreateException(Type notFoundType, Type requesterType = null)
			: base(string.Format(
				requesterType == null
					? ErrorMessageFormat
					: ErrorMessageWithRequesterFormat,
				notFoundType,
				requesterType))
		{
			NotFoundType = notFoundType;
			RequesterType = requesterType;
		}
	}
}
