using System;
using UnityEngine;

namespace RedCatEngine.DependencyInjection.Exceptions
{
	public class GameObjectNotContainComponentException : Exception
	{
		public readonly Type NotFoundComponentType;
		private const string ErrorMessageFormat = "Not found component Type {0} on gameObject {1} on component type not MonoConstruct";

		public GameObjectNotContainComponentException(Type notFoundComponentType, GameObject gameObject)
			: base(string.Format(ErrorMessageFormat, notFoundComponentType, gameObject))
		{
			NotFoundComponentType = notFoundComponentType;
		}
	}
}