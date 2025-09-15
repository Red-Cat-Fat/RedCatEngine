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
		[SerializeReference]
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

		public float GetValue(IGetterApplicationContainer getterContainer)
		{
			var x = FilterXAxis(GetXValue(getterContainer));
			return _curve.Evaluate(x);
		}

		private float GetXValue(IGetterApplicationContainer getterContainer)
			=> _xValue.GetValue(getterContainer);

		protected abstract float FilterXAxis(float getXValue);
	}
}