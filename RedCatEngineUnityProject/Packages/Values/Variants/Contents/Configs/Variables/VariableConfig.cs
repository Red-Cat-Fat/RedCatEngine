using RedCatEngine.Configs;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Configs.Variables
{
	[CreateAssetMenu(menuName = "Configs/Values/Variable", fileName = nameof(VariableConfig))]
	public class VariableConfig : BaseConfig
	{
		public float DefaultValue;
	}
}