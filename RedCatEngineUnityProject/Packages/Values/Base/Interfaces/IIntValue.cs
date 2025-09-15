using RedCatEngine.Values.Services;

namespace RedCatEngine.Values.Base.Interfaces
{
    public interface IIntValue : IValue<int>
    {
        int GetValue(ValueCalculationService getterContainer) 
            => getterContainer.GetValue(this);
    }
}