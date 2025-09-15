using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Constants
{
	[Serializable]
	[SRName("Common/Constant Int")]
	public class ConstantIntValue : IIntValue
	{
		[SerializeField]
		private int _value;

		public ConstantIntValue()
		{
			_value = 0;
		}

		public ConstantIntValue(int value)
		{
			_value = value;
		}

		public static ConstantIntValue Zero
			=> new() { _value = 0 };

		public static ConstantIntValue One
			=> new() { _value = 1 };

		public int GetValue(IGetterApplicationContainer getterContainer)
			=> _value;
	}
}