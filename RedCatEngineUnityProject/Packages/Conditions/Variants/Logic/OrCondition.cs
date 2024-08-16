using System;
using System.Linq;
using RedCatEngine.Conditions.Base;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Conditions.Variants.Logic
{
	[Serializable]
	[SRName("Logic/Or")]
	public class OrCondition : ICondition
	{
		[SR]
		[SerializeReference]
		public ICondition[] Conditions;

		public bool Check(IApplicationContainer applicationContainer)
		{
			return Conditions.Any(condition => condition.Check(applicationContainer));
		}
	}
}