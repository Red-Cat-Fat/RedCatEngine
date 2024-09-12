using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Constants
{
	[Serializable]
	[SRName("Constants/Constant Float")]
	public class ConstantFloatValue : IFloatValue
	{
		[SerializeField]
		private float _value;

		public float GetValue(IApplicationContainer applicationContainer)
			=> _value;

		public ConstantFloatValue()
		{
			_value = 0;
		}

		public ConstantFloatValue(float value)
		{
			_value = value;
		}
	}
}