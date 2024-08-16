using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Conditions.Base
{
	[CreateAssetMenu(menuName = "Configs/Condition/Config container", fileName = nameof(ConditionConfig))]
	public abstract class ConditionConfig : BaseConfig
	{
		[SR]
		[SerializeReference]
		private ICondition _condition;

		public bool Check(IApplicationContainer applicationContainer)
			=> _condition.Check(applicationContainer);
	}
}