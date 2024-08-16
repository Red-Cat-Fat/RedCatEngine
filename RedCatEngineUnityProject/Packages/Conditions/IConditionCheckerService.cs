using RedCatEngine.Conditions.Base;

namespace RedCatEngine.Conditions
{
	public interface IConditionCheckerService
	{
		bool Check(ICondition condition);
	}
}