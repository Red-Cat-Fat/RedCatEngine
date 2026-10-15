using System;
using System.Linq;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Curves
{
	[Serializable]
	public abstract class CurveValue : IFloatValue
	{
		[SerializeField]
		private AnimationCurve _curve;

		[SerializeField]
		protected float MinXValue;
		[SerializeField]
		protected float MaxXValue;
		[SR]
		[SerializeReference]
		private IFloatValue _xValue = ConstantFloatValue.Zero;

		protected CurveValue()
		{
		}

		protected CurveValue(AnimationCurve curve, IFloatValue xValue)
		{
			_xValue = xValue;
			_curve = curve;
			MinXValue = _curve.keys.First().time;
			MaxXValue = _curve.keys.Last().time;
		}

		/// <summary>
		/// The lower bound of X: <see cref="MinXValue"/>, or the time of the first key when no valid range is set.
		/// </summary>
		protected float RangeMin
			=> HasCustomRange ? MinXValue : _curve.keys.First().time;

		/// <summary>
		/// The upper bound of X: <see cref="MaxXValue"/>, or the time of the last key when no valid range is set.
		/// </summary>
		protected float RangeMax
			=> HasCustomRange ? MaxXValue : _curve.keys.Last().time;

		private bool HasCustomRange
			=> MaxXValue > MinXValue;

		public float GetValue(IGetterApplicationContainer getterContainer)
		{
			if (_curve == null || _curve.length == 0)
				return 0f;

			var x = FilterXAxis(GetXValue(getterContainer));
			return _curve.Evaluate(x);
		}

		private float GetXValue(IGetterApplicationContainer getterContainer)
			=> _xValue.GetValue(getterContainer);

		protected abstract float FilterXAxis(float getXValue);
	}
}