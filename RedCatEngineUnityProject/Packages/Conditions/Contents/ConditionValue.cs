using System;
using RedCatEngine.Conditions.Base;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Conditions.Contents
{
	[Serializable]
	[SRName("Common/Condition value")]
	public class ConditionValue : IBoolValue
	{
		[SR]
		[SerializeReference]
		private ICondition _condition;

		public bool GetValue(IGetterApplicationContainer getterContainer)
			=> _condition.Check(getterContainer);
	}
}