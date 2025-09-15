using System;
using RedCatEngine.Values.Base.BaseRealisations;
using RedCatEngine.Values.Base.Interfaces;
using SerializeReferenceEditor;

namespace RedCatEngine.Values.Variants.Contents.Configs.Links
{
	[Serializable]
	[SRName("ConfigLink/ConfigLink Bool")]
	public class BoolConfigLinkValue : BaseConfigLinkValue<bool>, IBoolValue
	{
	}
}