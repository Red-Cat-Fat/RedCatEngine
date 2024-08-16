using System;
using JetBrains.Annotations;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.Values.Base;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Logics
{
	[Serializable]
	[SRName("Logic/Conditional Float")]
	public class ConditionalFloatValue : IFloatValue
	{
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

		public float GetValue(IApplicationContainer applicationContainer)
		{
			return _checkValue.GetValue(applicationContainer)
				? _value.GetValue(applicationContainer)
				: _alternativeValue.GetValue(applicationContainer);
		}
	}
}