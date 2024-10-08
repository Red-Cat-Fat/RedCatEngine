﻿using System;

namespace RedCatEngine.DependencyInjection.Exceptions
{
	public class NotFountInjectAttributeForConstructorException : Exception
	{
		private const string ErrorMessageFormat = "Not found InjectAttribute for construcor in Type {0}";
		public Type NotFoundType { get; }
		public NotFountInjectAttributeForConstructorException(Type notFoundType)
			: base(string.Format(ErrorMessageFormat, notFoundType))
		{
			NotFoundType = notFoundType;
		}
	}
}