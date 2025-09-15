using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.Values.Base.Interfaces;
using RedCatEngine.Values.Variants.Contents.Constants;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Rounders
{
	public enum RoundType
	{
		Nearest,
		Ceiling
	}
	[Serializable]
	[SRName("Converters/Float to Int")]
	public class RoundFloatToIntValue : IIntValue
	{
		[SerializeField]
		private RoundType _roundType;
		[SR]
		[SerializeReference]
		private IFloatValue _value = new ConstantFloatValue(0);

		public int GetValue(IGetterApplicationContainer getterContainer)
		{
			var floatValue = _value.GetValue(getterContainer);
			return _roundType switch
			{
				RoundType.Nearest => (int)Math.Round(floatValue),
				RoundType.Ceiling => (int)Math.Ceiling(floatValue),
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}