using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Constants
{
	[Serializable]
	[SRName("Constants/Constant Bool")]
	public class ConstantBoolValue : IBoolValue
	{
		public static ConstantBoolValue True
			=> new() { _value = true };
		public static ConstantBoolValue False
			=> new() { _value = false };

		[SerializeField]
		private bool _value;

		public bool GetValue(IApplicationContainer applicationContainer)
			=> _value;
	}
}