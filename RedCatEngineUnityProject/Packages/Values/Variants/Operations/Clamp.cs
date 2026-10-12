using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Operations
{
	[Serializable]
	[SRName("Operations/Clamp")]
	public class Clamp : IFloatValue
	{
		[Header(
			"Возвращает _returnValue, если значение в интервале между _minValue и _maxValue. "
			+ "\nВ противном случае возвращает _minValue или _maxValue, если выходит за рамки."
		)]
		[SR]
		[SerializeReference]
		private IFloatValue _returnValue = ConstantFloatValue.Zero;

		[SR] [SerializeReference] private IFloatValue _minValue = ConstantFloatValue.Zero;
		[SR] [SerializeReference] private IFloatValue _maxValue = ConstantFloatValue.One;

		public float GetValue(IGetterApplicationContainer getterContainer)
		{
			var currentValue = _returnValue.GetValue(getterContainer);
			var minValue = _minValue.GetValue(getterContainer);
			var maxValue = _maxValue.GetValue(getterContainer);
			return Mathf.Clamp(currentValue, minValue, maxValue);;
		}
	}
}