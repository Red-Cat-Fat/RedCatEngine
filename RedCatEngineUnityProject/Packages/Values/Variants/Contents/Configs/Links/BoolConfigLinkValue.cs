using System;
using RedCatEngine.Values.Base;
using SerializeReferenceEditor;

namespace RedCatEngine.Values.Variants.Contents.Configs.Links
{
	[Serializable]
	[SRName("ConfigLink/ConfigLink Bool")]
	public class BoolConfigLinkValue : BaseConfigLinkValue<bool>, IBoolValue { }
}