namespace RedCatEngine.DependencyInjection.Specials.Providers.Waiters
{
	/// <summary>
	/// Если кто-то реализует данный интерфейс, то он будет получать объекты указанных типов в случае регистрации их в контейнере как Single-объект
	/// </summary>
	public interface ISingleWaiter : IWaiter
	{
	}

	/// <summary>
	/// Если кто-то реализует данный интерфейс, то он будет получать объекты указанных типов в случае регистрации их в контейнере как Single-объект
	/// </summary>
	public interface ISingleWaiter<in TWaitType> : ISingleWaiter where TWaitType : class
	{
		void IWaiter.Attach(object waitType)
		{
			if (waitType is TWaitType typed)
				Attach(typed);
		}

		void Attach(TWaitType waitType);
	}
}