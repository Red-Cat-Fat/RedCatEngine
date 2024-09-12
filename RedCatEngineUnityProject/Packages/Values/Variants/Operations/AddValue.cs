using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Operations
{
	[Serializable]
	[SRName("Operations/Sum")]
	public class AddValue : IFloatValue
	{
		[SR]
		[SerializeReference]
		private IFloatValue[] _values = {};

		public float GetValue(IApplicationContainer applicationContainer)
		{
			if (_values.Length == 0)
				return 0;

			var resultValue = 0f;
			foreach (var floatValue in _values)
			{
				resultValue += floatValue.GetValue(applicationContainer);
			}

			return resultValue;
		}
	}
}