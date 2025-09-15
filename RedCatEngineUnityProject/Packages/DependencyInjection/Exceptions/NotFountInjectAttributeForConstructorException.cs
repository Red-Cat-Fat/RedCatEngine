using System;

namespace RedCatEngine.DependencyInjection.Exceptions
{
	public class NotFountInjectAttributeForConstructorException<TAttribute> : Exception where TAttribute : Attribute
	{
		private const string ErrorMessageFormat = "Type {0} not contain InjectAttribute {1} for construcor";

		public NotFountInjectAttributeForConstructorException(Type notFoundType)
			: base(string.Format(ErrorMessageFormat, notFoundType, typeof(TAttribute)))
		{
			NotFoundType = notFoundType;
		}

		public Type NotFoundType { get; }
	}
}