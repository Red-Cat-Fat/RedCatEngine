using System;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Curves
{
	[Serializable]
	[SRName("Curve/Clamp")]
	public class ClampCurveValue : CurveValue
	{
		public ClampCurveValue()
		{
		}

		public ClampCurveValue(AnimationCurve curve, IFloatValue xValue) : base(curve, xValue)
		{
		}

		protected override float FilterXAxis(float getXValue)
			=> Mathf.Clamp(
				getXValue,
				RangeMin,
				RangeMax);
	}
}