using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Operations
{
	[Serializable]
	[SRName("Operations/Division")]
	public class DivisionValue : IFloatValue
	{
		[Header("Result = Base / Diver")]
		[SR]
		[SerializeReference]
		private IFloatValue _base;
		[SR]
		[SerializeReference]
		private IFloatValue _diver;

		public float GetValue(IGetterApplicationContainer getterContainer)
		{
			var baseValue = _base.GetValue(getterContainer);
			var diverValue = _diver.GetValue(getterContainer);

			if (diverValue != 0)
				return baseValue / diverValue;

			Debug.LogError("Division by zero");
			return 0;
		}
	}
}