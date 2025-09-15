using System;
using RedCatEngine.Conditions.Base;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Conditions.Variants
{
	[Serializable]
	[SRName("Links/Condition Config")]
	public class ConditionConfigLink : ICondition
	{
		[SerializeField]
		private ConditionConfig _conditionConfig;

		public bool Check(IGetterApplicationContainer getter)
		{
			return _conditionConfig.Check(getter);
		}
	}
}