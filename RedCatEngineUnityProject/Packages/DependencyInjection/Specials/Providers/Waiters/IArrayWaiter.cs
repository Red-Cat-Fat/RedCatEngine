using System.Collections.Generic;

namespace RedCatEngine.DependencyInjection.Specials.Providers.Waiters
{
	/// <summary>
	/// Если кто-то реализует данный интерфейс, то он будет получать объекты указанных типов в случае регистрации их в контейнере как часть элементов массива
	/// </summary>
	public interface IArrayWaiter : IWaiter
	{
	}

	/// <summary>
	/// Если кто-то реализует данный интерфейс, то он будет получать объекты указанных типов в случае регистрации их в контейнере как часть элементов массива
	/// </summary>
	public interface IArrayWaiter<in TWaitType> : IArrayWaiter where TWaitType : class
	{
		void IWaiter.Attach(object waitType)
		{
			if (waitType is TWaitType[] typed)
				Attach(typed);
			if (waitType is TWaitType type)
				Attach(type);
		}

		void Attach(IEnumerable<TWaitType> waitTypes);

		void Attach(TWaitType waitType);
	}
}