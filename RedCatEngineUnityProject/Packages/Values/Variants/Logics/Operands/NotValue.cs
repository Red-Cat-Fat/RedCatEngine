using System;
using RedCatEngine.Values.Base.Interfaces;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;
using IGetterApplicationContainer =
	RedCatEngine.DependencyInjection.Containers.Interfaces.Application.IGetterApplicationContainer;

namespace RedCatEngine.Values.Variants.Logics.Operands
{
	[Serializable]
	[SRName("Logic/Not")]
	public class NotValue : IBoolValue
	{
		[SR]
		[SerializeReference]
		private IBoolValue _value = ConstantBoolValue.False;

		public bool GetValue(IGetterApplicationContainer getterContainer)
			=> !_value.GetValue(getterContainer);
	}
}