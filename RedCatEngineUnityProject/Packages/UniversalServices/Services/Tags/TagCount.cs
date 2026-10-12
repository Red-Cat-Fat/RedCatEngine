using System;
using RedCatEngine.CommonServices.Containers.Storages.Tags;

namespace RedCatEngine.CommonServices.Services.Tags
{
	[Serializable]
	public struct TagCount
	{
		public uint TagConfigId;
		public int Count;

		public TagCount(TagConfig tag, int count)
		{
			TagConfigId = tag.Id;
			Count = count;
		}
	}
}