using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base;
using RedCatEngine.Values.Variants.Contents;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

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

		public bool GetValue(IApplicationContainer applicationContainer)
			=> Math.Abs(_baseComparison.GetValue(applicationContainer) - _otherValue.GetValue(applicationContainer)) < _tolerance.GetValue(applicationContainer);
	}
}