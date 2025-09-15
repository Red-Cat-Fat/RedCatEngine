using System;
using RedCatEngine.Values.Base.Interfaces;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;
using IGetterApplicationContainer =
	RedCatEngine.DependencyInjection.Containers.Interfaces.Application.IGetterApplicationContainer;

namespace RedCatEngine.Values.Variants.Logics.Comparisons
{
	[Serializable]
	[SRName("Comparisons/Equals")]
	public class EqualsValue : IBoolValue
	{
		[SR]
		[SerializeReference]
		private IFloatValue _baseComparison = new ConstantFloatValue(0);

		[SR]
		[SerializeReference]
		private IFloatValue _otherValue = new ConstantFloatValue(0);

		[SR]
		[SerializeReference]
		private IFloatValue _tolerance = new ConstantFloatValue(0);

		public bool GetValue(IGetterApplicationContainer getterContainer)
			=> Math.Abs(_baseComparison.GetValue(getterContainer) - _otherValue.GetValue(getterContainer))
				< _tolerance.GetValue(getterContainer);
	}
}