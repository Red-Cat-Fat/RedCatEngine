using System;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;
using IGetterApplicationContainer =
	RedCatEngine.DependencyInjection.Containers.Interfaces.Application.IGetterApplicationContainer;

namespace RedCatEngine.Values.Variants.Contents.Constants
{
	[Serializable]
	[SRName("Common/Constant Float")]
	public class ConstantFloatValue : IFloatValue
	{
		[SerializeField] private float _value;

		public ConstantFloatValue()
		{
			_value = 0;
		}

		public ConstantFloatValue(float value)
		{
			_value = value;
		}

		public static ConstantFloatValue Zero
			=> new() { _value = 0 };

		public static ConstantFloatValue One
			=> new() { _value = 1f };

		public float GetValue(IGetterApplicationContainer getterContainer)
			=> _value;

		public override string ToString()
		{
			return _value + "(const)";
		}
	}
}