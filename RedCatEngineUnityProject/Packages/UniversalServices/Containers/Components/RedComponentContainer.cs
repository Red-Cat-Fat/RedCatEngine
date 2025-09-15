using System;
using System.Collections.Generic;
using RedCatEngine.CommonServices.Containers.Storages.TypedStorage;

namespace RedCatEngine.CommonServices.Containers.Components
{
	public class RedComponentContainer : ConditionTypedStorage<IRedComponent>, IRedComponentContainer
	{
		public bool TryGetRedComponent<TRedComponent>(out TRedComponent result) where TRedComponent : IRedComponent
			=> TryGet(out result);

		public TRedComponent GetRedComponent<TRedComponent>() where TRedComponent : IRedComponent
		{
			if (TryGet<TRedComponent>(out var result))
				return result;

			throw new Exception("Not found component" + typeof(TRedComponent));
		}

		public IEnumerable<TRedComponent> GetRedComponents<TRedComponent>() where TRedComponent : IRedComponent
		{
			return TryGets<TRedComponent>(out var result) ? result : ArraySegment<TRedComponent>.Empty;
		}

		public new void Add<TRedComponent>(TRedComponent item) where TRedComponent : IRedComponent
		{
			base.Add(item);
		}

		public void Remove<TRedComponent>() where TRedComponent : IRedComponent
		{
			base.Remove<TRedComponent>();
		}

		public bool IsContains<TRedComponent>() where TRedComponent : IRedComponent
		{
			return IsContain<TRedComponent>();
		}
	}
}