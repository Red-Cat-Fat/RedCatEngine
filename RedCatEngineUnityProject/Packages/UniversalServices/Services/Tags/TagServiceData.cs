using System;
using System.Collections.Generic;
using System.Linq;

namespace RedCatEngine.CommonServices.Services.Tags
{
	[Serializable]
	public class TagServiceData
	{
		public TagCount[] Tags;

		public TagServiceData()
		{
			Tags = Array.Empty<TagCount>();
		}

		public TagServiceData(IEnumerable<TagCount> tagCounts)
		{
			Tags = tagCounts.ToArray();
		}
	}
}