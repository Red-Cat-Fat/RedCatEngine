using System;
using System.Linq;
using UnityEngine;

namespace RedCatEngine.CommonServices.Extensions
{
	public static class AnimationCurveExtensions
	{
		public enum GetCurveType
		{
			Duplication = 1,
			Clamp = 2
		}

		public static float GetValue(
			this AnimationCurve animationCurve,
			float x,
			GetCurveType curveType = GetCurveType.Clamp
		)
		{
			var minXValue = animationCurve.keys.First().time;
			var maxXValue = animationCurve.keys.Last().time;
			switch (curveType)
			{
				case GetCurveType.Duplication:
					var absMax = Mathf.Abs(minXValue);
					var absMin = Mathf.Abs(maxXValue);

					while (x > maxXValue)
						x -= absMax;
					while (x < minXValue)
						x += absMin;

					return animationCurve.Evaluate(x);
				case GetCurveType.Clamp:
					x = Mathf.Clamp(x, minXValue, maxXValue);
					return animationCurve.Evaluate(x);
				default:
					throw new ArgumentOutOfRangeException(nameof(curveType), curveType, null);
			}
		}
	}
}