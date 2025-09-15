using System;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Configs.Variables
{
	[Serializable]
	public class OverrideVariableConfigCalculateValue
	{
		[HideInInspector]
		public string DebugName;
		public VariableConfig Variable;
		[SR]
		[SerializeReference]
		public IFloatValue Value;
	}
}