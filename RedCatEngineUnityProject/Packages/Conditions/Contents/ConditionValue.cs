using System;
using RedCatEngine.Conditions.Base;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Conditions.Contents
{
	[Serializable]
	[SRName("Condition")]
	public class ConditionValue : IBoolValue
	{
		[SR]
		[SerializeReference]
		private ICondition _condition;

		public bool GetValue(IApplicationContainer applicationContainer)
			=> _condition.Check(applicationContainer);
	}
}