using System.Collections.Generic;

namespace RedCatEngine.DependencyInjection.Specials.Providers
{
	public class ArrayProvider<TTypeProvide> : IArrayProvider<TTypeProvide>, IArrayWaiter<TTypeProvide> where TTypeProvide : class
	{
		private readonly List<TTypeProvide> _instances = new();

		public bool TryGet(out TTypeProvide[] instance)
		{
			instance = _instances.ToArray();
			return instance != null;
		}

		public void Attach(TTypeProvide waitType)
			=> _instances.Add(waitType);

		public void Attach(IEnumerable<TTypeProvide> waitTypes)
			=> _instances.AddRange(waitTypes);
	}
}