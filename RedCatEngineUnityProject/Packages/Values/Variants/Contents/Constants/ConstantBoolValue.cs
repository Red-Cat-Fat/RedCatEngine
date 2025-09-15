using System;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;
using IGetterApplicationContainer =
	RedCatEngine.DependencyInjection.Containers.Interfaces.Application.IGetterApplicationContainer;

namespace RedCatEngine.Values.Variants.Contents.Constants
{
	[Serializable]
	[SRName("Common/Constant Bool")]
	public class ConstantBoolValue : IBoolValue
	{
		[SerializeField]
		private bool _value;
		public static ConstantBoolValue True
			=> new() { _value = true };
		public static ConstantBoolValue False
			=> new() { _value = false };

		public bool GetValue(IGetterApplicationContainer getterContainer)
			=> _value;
	}
}