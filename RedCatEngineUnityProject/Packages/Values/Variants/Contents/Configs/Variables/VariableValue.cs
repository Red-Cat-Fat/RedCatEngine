using System;
using RedCatEngine.Values.Base.BaseRealisations;
using RedCatEngine.Values.Base.Interfaces;
using RedCatEngine.Values.Services;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Configs.Variables
{
	[Serializable]
	[SRName("Common/Global Variable")]
	public class VariableValue : BaseServiceGetterValue<VariableContainer, float>, IFloatValue
	{
		[SerializeField]
		private VariableConfig _variable;

		protected override float GetValueFromServices(VariableContainer single)
			=> single.GetValue(_variable);
	}
}