using System;
using RedCatEngine.Conditions.Base;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Conditions.Variants
{
	[Serializable]
	[SRName("Value Condition")]
	public class ReValueCondition : ICondition
	{
		[SR]
		[SerializeReference]
		private IBoolValue _resultValue;

		public bool Check(IApplicationContainer applicationContainer)
			=> _resultValue.GetValue(applicationContainer);
	}
}