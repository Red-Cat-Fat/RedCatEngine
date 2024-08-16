using System;
using RedCatEngine.Conditions.Base;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Conditions.Variants.Logic
{
	[Serializable]
	[SRName("Logic/Not")]
	public class NotCondition : ICondition
	{
		[SR]
		[SerializeReference]
		public ICondition Condition;

		public bool Check(IApplicationContainer applicationContainer)
		{
			return !Condition.Check(applicationContainer);
		}
	}
}