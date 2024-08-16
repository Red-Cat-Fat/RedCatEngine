using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.Values.Base;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Operations
{
	[Serializable]
	[SRName("Operations/Multiply")]
	public class MultiplyValue : IFloatValue
	{
		[SR]
		[SerializeReference]
		private IFloatValue[] _values = {};

		public float GetValue(IApplicationContainer applicationContainer)
		{
			if (_values.Length == 0)
				return 0;

			var resultValue = 1f;
			foreach (var floatValue in _values)
			{
				resultValue *= floatValue.GetValue(applicationContainer);
			}

			return resultValue;
		}
	}
}