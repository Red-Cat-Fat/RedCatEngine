using System;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Curves
{
	[Serializable]
	[SRName("Curve/Repeat")]
	public class DuplicationCurveValue : CurveValue
	{
		public DuplicationCurveValue()
		{
		}

		public DuplicationCurveValue(AnimationCurve curve, IFloatValue xValue) : base(curve, xValue)
		{
		}

		protected override float FilterXAxis(float getXValue)
		{
			var min = RangeMin;
			var length = RangeMax - min;
			if (length <= 0f)
				return min;

			return min + Mathf.Repeat(getXValue - min, length);
		}
	}
}