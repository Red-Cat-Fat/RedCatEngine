using System;
using RedCatEngine.Values.Base.Interfaces;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Curves
{
	[Serializable]
	public class ClampCurveValue : CurveValue
	{
		public ClampCurveValue() : base()
		{
		}

		public ClampCurveValue(AnimationCurve curve, IFloatValue xValue) : base(curve, xValue)
		{
		}

		protected override float FilterXAxis(float getXValue)
			=> Mathf.Clamp(
				getXValue,
				MinXValue,
				MaxXValue);
	}
}