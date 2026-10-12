using System;
using JetBrains.Annotations;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;
using IGetterApplicationContainer =
	RedCatEngine.DependencyInjection.Containers.Interfaces.Application.IGetterApplicationContainer;

namespace RedCatEngine.Values.Variants.Logics
{
	[Serializable]
	[SRName("Logic/Conditional Float")]
	public class ConditionalFloatValue : IFloatValue
	{
		[Header("Проверяет _checkValue\nВозвращает _value если TRUE \nили _alternativeValue если FALSE")]
		[SR]
		[SerializeReference]
		[UsedImplicitly]
		private IBoolValue _checkValue;
		[SR]
		[SerializeReference]
		[UsedImplicitly]
		private IFloatValue _value;
		[SR]
		[SerializeReference]
		[UsedImplicitly]
		private IFloatValue _alternativeValue;

		public float GetValue(IGetterApplicationContainer getterContainer)
		{
			return _checkValue.GetValue(getterContainer)
				? _value.GetValue(getterContainer)
				: _alternativeValue.GetValue(getterContainer);
		}
	}
}