using System;
using RedCatEngine.Conditions.Base;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Conditions.Variants
{
	[Serializable]
	[SRName("Logic Value Condition")]
	public class CheckValueCondition : ICondition
	{
		[SR]
		[SerializeReference]
		private IBoolValue _resultValue;

		public bool Check(IGetterApplicationContainer getter)
			=> _resultValue.GetValue(getter);
	}
}