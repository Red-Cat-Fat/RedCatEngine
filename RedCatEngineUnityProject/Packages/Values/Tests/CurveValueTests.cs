using System.Reflection;
using NUnit.Framework;
using RedCatEngine.Values.Variants.Contents.Constants;
using RedCatEngine.Values.Variants.Contents.Curves;
using UnityEngine;

namespace RedCatEngine.Values.Tests
{
	public class CurveValueTests
	{
		private static AnimationCurve MakeLinearCurve()
			=> AnimationCurve.Linear(0f, 0f, 10f, 10f);

		// a value created by the [SR] picker: default constructor, no range set
		private static TCurve MakeLikeInspector<TCurve>(AnimationCurve curve, float x) where TCurve : CurveValue, new()
		{
			var value = new TCurve();
			typeof(CurveValue)
				.GetField("_curve", BindingFlags.Instance | BindingFlags.NonPublic)
				.SetValue(value, curve);
			typeof(CurveValue)
				.GetField("_xValue", BindingFlags.Instance | BindingFlags.NonPublic)
				.SetValue(value, new ConstantFloatValue(x));
			return value;
		}

		[Test]
		public void GivenMaxXValue_ThenItIsSerializedAsField()
		{
			var field = typeof(CurveValue).GetField("MaxXValue", BindingFlags.Instance | BindingFlags.NonPublic);

			Assert.IsNotNull(field.GetCustomAttribute<SerializeField>(), "MaxXValue must be [SerializeField]");
			Assert.IsNull(field.GetCustomAttribute<SerializeReference>(), "MaxXValue must not be [SerializeReference]");
		}

		[Test]
		public void GivenClampCurve_WhenXAboveRange_ThenReturnsLastKeyValue()
		{
			var value = new ClampCurveValue(MakeLinearCurve(), new ConstantFloatValue(20f));

			Assert.AreEqual(10f, value.GetValue(null), 0.001f);
		}

		[Test]
		public void GivenRepeatCurve_WhenXAboveRange_ThenWrapsIntoRange()
		{
			var value = new DuplicationCurveValue(MakeLinearCurve(), new ConstantFloatValue(15f));

			Assert.AreEqual(5f, value.GetValue(null), 0.001f);
		}

		[Test]
		public void GivenRepeatCurve_WhenXBelowRange_ThenWrapsIntoRange()
		{
			var value = new DuplicationCurveValue(MakeLinearCurve(), new ConstantFloatValue(-3f));

			Assert.AreEqual(7f, value.GetValue(null), 0.001f);
		}

		[Test]
		public void GivenInspectorRepeatCurveWithoutRange_WhenXAboveRange_ThenUsesCurveKeys()
		{
			var value = MakeLikeInspector<DuplicationCurveValue>(MakeLinearCurve(), 15f);

			Assert.AreEqual(5f, value.GetValue(null), 0.001f);
		}

		[Test]
		public void GivenInspectorClampCurveWithoutRange_WhenXAboveRange_ThenUsesCurveKeys()
		{
			var value = MakeLikeInspector<ClampCurveValue>(MakeLinearCurve(), 20f);

			Assert.AreEqual(10f, value.GetValue(null), 0.001f);
		}
	}
}
