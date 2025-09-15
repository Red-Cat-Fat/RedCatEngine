using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Operations
{
	[Serializable]
	[SRName("Operations/Subtract")]
	public class SubtractionValue : IFloatValue
	{
		[Header("Result = Base - Subtracted")]
		[SR]
		[SerializeReference]
		private IFloatValue _base;
		[SR]
		[SerializeReference]
		private IFloatValue _subtracted;

		public float GetValue(IGetterApplicationContainer getterContainer)
		{
			var baseValue = _base.GetValue(getterContainer);
			var diverValue = _subtracted.GetValue(getterContainer);
			return baseValue - diverValue;
		}
	}
}