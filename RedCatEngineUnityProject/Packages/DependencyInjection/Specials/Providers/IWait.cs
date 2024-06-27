namespace RedCatEngine.DependencyInjection.Specials.Providers
{
	public interface IWait
	{
		void Attach(object waitType);
	}
	public interface IWait<in TWaitType> : IWait where TWaitType : class
	{
		void Attach(TWaitType waitType);

		void IWait.Attach(object waitType)
		{
			if (waitType is TWaitType typed)
				Attach(typed);
		}
	}
}