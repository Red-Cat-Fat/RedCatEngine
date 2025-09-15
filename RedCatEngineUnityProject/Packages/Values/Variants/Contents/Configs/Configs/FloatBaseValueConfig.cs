using RedCatEngine.Values.Base.BaseRealisations;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Configs.Configs
{
	[SRHidden]
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