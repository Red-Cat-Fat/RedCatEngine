using RedCatEngine.Conditions.Base;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using UnityEngine;

namespace RedCatEngine.Conditions
{
	public class ConditionCheckerService : IConditionCheckerService
	{
		private readonly IApplicationContainer _applicationContainer;

		public ConditionCheckerService(IApplicationContainer applicationContainer)
		{
			_applicationContainer = applicationContainer;
		}

		public bool Check(ICondition condition)
		{
			Debug.Log($"[ConditionApplierService] Check condition: {condition}");
			return condition.Check(_applicationContainer);
		}
	}
}