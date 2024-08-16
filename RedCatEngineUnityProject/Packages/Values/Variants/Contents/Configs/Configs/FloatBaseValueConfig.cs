using RedCatEngine.Values.Base;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Configs.Configs
{
	[CreateAssetMenu(menuName = "Configs/Values/Float", fileName = nameof(FloatBaseValueConfig))]
	public class FloatBaseValueConfig : BaseValueConfig<float>, IFloatValue
	{
		[SR]
		[SerializeReference]
		private IFloatValue _value;

		protected override IValue<float> ReturnValue
			=> _value;
	}
}