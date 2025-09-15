using System;
using RedCatEngine.Values.Base.Interfaces;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Curves
{
	[Serializable]
	public class DuplicationCurveValue : CurveValue
	{
		public DuplicationCurveValue() : base()
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