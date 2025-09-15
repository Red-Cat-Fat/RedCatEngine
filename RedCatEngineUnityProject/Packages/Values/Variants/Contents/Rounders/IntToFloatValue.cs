using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Rounders
{
	[Serializable]
	[SRName("Converters/Int to Float")]
	public class IntToFloatValue : IFloatValue
	{
		[SR]
		[SerializeReference]
		private IIntValue _value = new ConstantIntValue(0);

		public float GetValue(IGetterApplicationContainer getterContainer)
			=> _value.GetValue(getterContainer);
	}
}