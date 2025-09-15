using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Logics.Comparisons
{
	[Serializable]
	[SRName("Comparisons/InInterval")]
	public class InIntervalValue : IBoolValue
	{
		[SerializeField]
		private bool _isEquals;

		[SR]
		[SerializeReference]
		private IFloatValue _checkValue = new ConstantFloatValue(0);

		[SR]
		[SerializeReference]
		private IFloatValue _maxValue = new ConstantFloatValue(0);
		[SR]
		[SerializeReference]
		private IFloatValue _minValue = new ConstantFloatValue(0);

		[SR]
		[SerializeReference]
		private IFloatValue _tolerance = new ConstantFloatValue(Mathf.Epsilon);

		public InIntervalValue()
		{
		}

		public InIntervalValue(
			IFloatValue minValue,
			IFloatValue maxValue,
			IFloatValue checkValue,
			bool isEquals = true
		)
		{
			_isEquals = isEquals;
			_minValue = minValue;
			_maxValue = maxValue;
			_checkValue = checkValue;
		}

		public bool GetValue(IGetterApplicationContainer getterContainer)
		{
			var minValue = _minValue.GetValue(getterContainer);
			var maxValue = _maxValue.GetValue(getterContainer);
			var checkValue = _checkValue.GetValue(getterContainer);

			var tolerance = _tolerance.GetValue(getterContainer);

			return minValue < checkValue && checkValue < maxValue ||
				(_isEquals &&
					(Math.Abs(minValue - checkValue) < tolerance
						|| Math.Abs(maxValue - checkValue) < tolerance));
		}
	}
}