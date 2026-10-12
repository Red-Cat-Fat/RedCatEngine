using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using UnityEngine;

namespace RedCatEngine.Conditions.Base
{
	[Serializable]
	public abstract class BaseSingleServiceCondition<TServiceCheck> : ICondition, IBoolValue
	{
		[SerializeField] private bool _invert;

		public bool GetValue(IGetterApplicationContainer getterContainer) => Check(getterContainer);

		public bool Check(IGetterApplicationContainer getter)
		{
			if(!getter.TryGetSingle<TServiceCheck>(out var service))
				throw new Exception("Not found condition checker");

			var result = DoCheck(service);
			if(_invert)
				return !result;
			return result;
		}

		protected abstract bool DoCheck(TServiceCheck service);
	}
}
