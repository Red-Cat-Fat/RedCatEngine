using RedCatEngine.Values.Base;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.Values.Variants.Contents.Configs.Configs
{
	[CreateAssetMenu(menuName = "Configs/Values/Bool", fileName = nameof(BoolBaseValueConfig))]
	public class BoolBaseValueConfig : BaseValueConfig<bool>
	{
		[SR]
		[SerializeReference]
		private IBoolValue _value;

		protected override IValue<bool> ReturnValue
			=> _value;
	}
}