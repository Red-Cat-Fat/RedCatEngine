using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base;
using RedCatEngine.Values.Variants.Contents;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Logics.Comparisons
{
	[Serializable]
	[SRName("Comparisons/InInterval")]
	public class InIntervalValue : IBoolValue
	{
		[SR]
		[SerializeReference]
		private IFloatValue _minValue = new ConstantFloatValue(0);

		[SR]
		[SerializeReference]
		private IFloatValue _maxValue = new ConstantFloatValue(0);

		[SR]
		[SerializeReference]
		private IFloatValue _checkValue = new ConstantFloatValue(0);

		[SerializeField]
		private bool _isEquals;

		[SR]
		[SerializeReference]
		private IFloatValue _tolerance = new ConstantFloatValue(Mathf.Epsilon);

		public InIntervalValue()
		{
			
		}

		public InIntervalValue(IFloatValue minValue, IFloatValue maxValue, IFloatValue checkValue, bool isEquals = true)
		{
			_isEquals = isEquals;
			_minValue = minValue;
			_maxValue = maxValue;
			_checkValue = checkValue;
		}
		
		public bool GetValue(IApplicationContainer applicationContainer)
		{
			var minValue = _minValue.GetValue(applicationContainer);
			var maxValue = _maxValue.GetValue(applicationContainer);
			var checkValue = _checkValue.GetValue(applicationContainer);

			var tolerance = _tolerance.GetValue(applicationContainer);

			return minValue < checkValue && checkValue < maxValue ||
			       (_isEquals &&
			        (Math.Abs(minValue - checkValue) < tolerance 
			         || Math.Abs(maxValue - checkValue) < tolerance));
		}
	}
}