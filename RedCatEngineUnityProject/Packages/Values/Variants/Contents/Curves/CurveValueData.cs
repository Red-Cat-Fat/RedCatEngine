using System;
using RedCatEngine.Values.Base.Interfaces;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Curves
{
	internal enum CurveType
	{
		Duplication,
		Clamp
	}

	[Serializable]
	public class CurveValueData
	{
		[SerializeField]
		private AnimationCurve _curve;

		[SerializeField]
		private CurveType _curveType;
		[SR]
		[SerializeReference]
		private IFloatValue _xValue = ConstantFloatValue.Zero;

		public CurveValue Make()
		{
			return _curveType switch
			{
				CurveType.Duplication => new DuplicationCurveValue(_curve, _xValue),
				CurveType.Clamp => new ClampCurveValue(_curve, _xValue),
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}