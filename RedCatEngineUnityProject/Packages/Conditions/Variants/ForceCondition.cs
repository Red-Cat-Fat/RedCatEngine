using System;
using RedCatEngine.Conditions.Base;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Conditions.Variants
{
	[Serializable]
	[SRName("Force Condition")]
	public class ForceCondition : ICondition
	{
		public static ICondition True
			=> new ForceCondition { _forceValue = true };
		public static ICondition False
			=> new ForceCondition { _forceValue = false };

		[SerializeField]
		private bool _forceValue;

		public bool Check(IApplicationContainer applicationContainer)
			=> _forceValue;
	}
}