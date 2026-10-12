using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;

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
			return baseValue * multiplier;
		}
	}
}