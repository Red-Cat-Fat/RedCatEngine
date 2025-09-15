using RedCatEngine.CommonServices.Containers.Storages.CountedStorage;

namespace RedCatEngine.CommonServices.Containers.Storages.Items
{
	public interface IItemStorage<TItem> : ICountedStorage<TItem>
		where TItem : BaseItemConfig
	{
		int GetLevel(TItem item);
		void SetLevel(TItem item, int level);
	}
}