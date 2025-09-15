namespace RedCatEngine.CommonServices.Containers.Storages.TypedStorage
{
	public class ConditionTypedStorage<TBaseType> : TypedStorage
	{
		protected override void AddToDictionary<TType>(TType item)
		{
			if(item is TBaseType)
				base.AddToDictionary(item);
		}
	}
}