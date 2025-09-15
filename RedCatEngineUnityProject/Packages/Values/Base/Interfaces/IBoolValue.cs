using RedCatEngine.Values.Services;

namespace RedCatEngine.Values.Base.Interfaces
{
	public interface IBoolValue : IValue<bool>
	{
		bool GetValue(ValueCalculationService getterContainer) 
			=> getterContainer.GetValue(this);
	}
}