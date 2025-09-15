using RedCatEngine.Values.Services;

namespace RedCatEngine.Values.Base.Interfaces
{
	public interface IFloatValue : IValue<float>
	{
		float GetValue(ValueCalculationService getterContainer) 
			=> getterContainer.GetValue(this);
	}
}