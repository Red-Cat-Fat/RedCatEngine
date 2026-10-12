using System;
using JetBrains.Annotations;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Logics
{
	[Serializable]
	[SRName("Logic/Conditional Float (Multi)")]
	public class MultiConditionalFloatValue : IFloatValue
	{
		[Header("Возвращает первый выполненный кондишен по порядку")]
		public ConditionValue[] _conditionsValues;

		[SR] [SerializeReference] [UsedImplicitly]
		public IFloatValue DefaultValue = ConstantFloatValue.Zero;

		public float GetValue(IGetterApplicationContainer getterContainer)
		{
			foreach (var conditionsValue in _conditionsValues)
			{
				if (conditionsValue.CheckValue.GetValue(getterContainer))
					return conditionsValue.Value.GetValue(getterContainer);
			}

			return DefaultValue.GetValue(getterContainer);
		}

		[Serializable]
		public class ConditionValue
		{
			[SR] [SerializeReference] [UsedImplicitly]
			public IBoolValue CheckValue = ConstantBoolValue.False;

			[SR] [SerializeReference] [UsedImplicitly]
			public IFloatValue Value = ConstantFloatValue.Zero;
		}
	}
}