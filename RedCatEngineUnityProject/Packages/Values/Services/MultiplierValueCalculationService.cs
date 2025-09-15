using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using UnityEngine;

namespace RedCatEngine.Values.Services
{
	public class MultiplierValueCalculationService : ValueCalculationService
	{
		private readonly IFloatValue _multiplier;

		public MultiplierValueCalculationService(IGetterApplicationContainer getter, IFloatValue multiplier)
			: base(getter)
		{
			_multiplier = multiplier;
		}

		public override float GetValue(IFloatValue floatValue)
		{
			var baseValue = base.GetValue(floatValue);
			var multiplier = base.GetValue(_multiplier);
			Debug.LogFormat("Calculate miltiplier value: {0} * {1} = {2}", baseValue, multiplier, baseValue * multiplier );
			return baseValue * multiplier;
		}
	}
}