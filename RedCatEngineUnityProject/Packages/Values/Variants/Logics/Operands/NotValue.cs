using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base;
using RedCatEngine.Values.Variants.Contents;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Logics.Operands
{
	[Serializable]
	[SRName("Logic/Not")]
	public class NotValue : IBoolValue
	{
		[SR]
		[SerializeReference]
		private IBoolValue _value = ConstantBoolValue.False;

		public bool GetValue(IApplicationContainer applicationContainer)
			=> !_value.GetValue(applicationContainer);
	}
}