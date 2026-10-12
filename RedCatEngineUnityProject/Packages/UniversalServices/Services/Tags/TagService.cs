using System.Linq;
using RedCatEngine.CommonServices.Containers.Storages.Tags;
using RedCatEngine.CommonServices.Services.Logs;
using RedCatEngine.CommonServices.Services.Save.Realisations;
using RedCatEngine.DependencyInjection.Containers.Attributes;

namespace RedCatEngine.CommonServices.Services.Tags
{
	public class TagService : ITagService, ISaved
	{
		private readonly ILogService _logService;
		private readonly SaveSerializer<TagServiceData> _serializer = new();
		private readonly AllTagLinksConfig _tagList;
		private readonly ITagStorage _tagStorage;

		[Inject]
		public TagService(
			ITagStorage tagStorage,
			AllTagLinksConfig tagList,
			ILogService logService
		)
		{
			_tagStorage = tagStorage;
			_logService = logService.CreateTag<TagService>();
			_tagList = tagList;
		}

		public string SaveKey
			=> nameof(TagService);

		public string GetSerializedSaveData()
			=> _serializer.SaveDataFrom(
				new TagServiceData(
					_tagStorage
						.GetElements()
						.Select(
							tag
								=> new TagCount(
									tag,
									_tagStorage.GetCount(tag)
								)
						)
				)
			);

		public void Load(string serializedSaveData)
		{
			var save = _serializer.LoadDataFrom(serializedSaveData);
			foreach (var tagCount in save.Tags)
			{
				if (!_tagList.TryGetById(tagCount.TagConfigId, out var tag))
				{
					_logService.LogErrorFormat("Not found tag by ID {0}", tagCount.TagConfigId);
					continue;
				}
				_tagStorage.Add(tag, tagCount.Count);
			}
		}
	}
}