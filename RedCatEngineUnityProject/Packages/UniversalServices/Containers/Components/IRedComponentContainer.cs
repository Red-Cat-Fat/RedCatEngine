using System.Collections.Generic;

namespace RedCatEngine.CommonServices.Containers.Components
{
    public interface IRedComponentContainer
    {
        bool TryGetRedComponent<TRedComponent>(out TRedComponent result) where TRedComponent : IRedComponent;
        TRedComponent GetRedComponent<TRedComponent>() where TRedComponent : IRedComponent;
        IEnumerable<TRedComponent> GetRedComponents<TRedComponent>() where TRedComponent : IRedComponent;
        void Add<TRedComponent>(TRedComponent item) where TRedComponent : IRedComponent;
        void Remove<TRedComponent>() where TRedComponent : IRedComponent;
        bool IsContains<TRedComponent>() where TRedComponent : IRedComponent;

        TRedComponent GetOrCreateRedComponent<TRedComponent>() where TRedComponent : IRedComponent, new()
        {
            if (TryGetRedComponent(out TRedComponent result))
                return result;

            result = new TRedComponent();
            Add(result);
            return result;
        }
    }
}