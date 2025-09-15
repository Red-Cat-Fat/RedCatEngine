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
	[SRName("Logic/And")]
	public class AndValue : IBoolValue
	{
		[SR]
		[SerializeReference]
		private IBoolValue _left = ConstantBoolValue.False;

		[SR]
		[SerializeReference]
		private IBoolValue _right = ConstantBoolValue.False;

		public bool GetValue(IGetterApplicationContainer getterContainer)
			=> _left.GetValue(getterContainer) && _right.GetValue(getterContainer);
	}
}