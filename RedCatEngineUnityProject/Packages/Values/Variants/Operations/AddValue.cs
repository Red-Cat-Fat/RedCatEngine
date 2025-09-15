using System;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;
using IGetterApplicationContainer =
	RedCatEngine.DependencyInjection.Containers.Interfaces.Application.IGetterApplicationContainer;

namespace RedCatEngine.Values.Variants.Operations
{
	[Serializable]
	[SRName("Operations/Sum")]
	public class AddValue : IFloatValue
	{
		[SR]
		[SerializeReference]
		private IFloatValue[] _values = { };

		public float GetValue(IGetterApplicationContainer getterContainer)
		{
			if (_values.Length == 0)
				return 0;

			var resultValue = 0f;
			foreach (var floatValue in _values)
			{
				resultValue += floatValue.GetValue(getterContainer);
			}

			return resultValue;
		}
	}
}