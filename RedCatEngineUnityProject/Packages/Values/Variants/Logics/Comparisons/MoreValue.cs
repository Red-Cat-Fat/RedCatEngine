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
	[SRName("Comparisons/More")]
	public class MoreValue : IBoolValue
	{
		[SR]
		[SerializeReference]
		private IFloatValue _baseComparison = new ConstantFloatValue(0);

		[SR]
		[SerializeReference]
		private IFloatValue _otherValue = new ConstantFloatValue(0);

		public bool GetValue(IGetterApplicationContainer getterContainer)
			=> _baseComparison.GetValue(getterContainer) > _otherValue.GetValue(getterContainer);
	}
}