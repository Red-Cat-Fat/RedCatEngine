using System;

namespace RedCatEngine.DependencyInjection.Specials.Providers.Waiters
{
	/// <summary>
	/// Если кто-то реализует данный интерфейс, то он будет получать объекты указанных типов в случае регистрации их в контейнере
	/// </summary>
	public interface IWaiter
	{
		Type[] ExpectedTypes { get; }
		void Attach(object waitType);
	}
}