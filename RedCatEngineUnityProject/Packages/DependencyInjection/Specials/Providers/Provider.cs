namespace RedCatEngine.DependencyInjection.Specials.Providers
{
	public class Provider<TTypeProvide> : IProvider<TTypeProvide>, IWait<TTypeProvide> where TTypeProvide : class
	{
		private TTypeProvide _instance;

		public bool TryGet(out TTypeProvide instance)
		{
			instance = _instance;
			return instance != null;
		}

		public void Attach(TTypeProvide waitType)
		{
			_instance = waitType;
		}
	}
}