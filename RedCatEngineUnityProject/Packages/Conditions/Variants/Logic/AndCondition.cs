using System;
using System.Linq;
using RedCatEngine.Conditions.Base;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Conditions.Variants.Logic
{
	[Serializable]
	[SRName("Logic/And")]
	public class AndCondition : ICondition
	{
		[SR]
		[SerializeReference]
		public ICondition[] Conditions;

		public bool Check(IApplicationContainer applicationContainer)
		{
			return Conditions.All(condition => condition.Check(applicationContainer));
		}
	}
}