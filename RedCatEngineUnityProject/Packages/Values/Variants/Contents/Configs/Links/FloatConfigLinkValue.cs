using System;
using RedCatEngine.Values.Base;
using SerializeReferenceEditor;

namespace RedCatEngine.Values.Variants.Contents.Configs.Links
{
	[Serializable]
	[SRName("ConfigLink/ConfigLink Float")]
	public class FloatConfigLinkValue : BaseConfigLinkValue<float>, IFloatValue { }
}