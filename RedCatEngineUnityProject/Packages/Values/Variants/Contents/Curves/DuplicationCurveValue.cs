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
			var absMax = Mathf.Abs(MaxXValue);
			var absMin = Mathf.Abs(MinXValue);

			while (getXValue > MaxXValue)
				getXValue -= absMax;
			while (getXValue < MinXValue)
				getXValue += absMin;

			return getXValue;
		}
	}
}