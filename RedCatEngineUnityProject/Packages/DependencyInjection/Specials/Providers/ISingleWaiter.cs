using System.Collections.Generic;

namespace RedCatEngine.DependencyInjection.Specials.Providers
{
	public interface IWaiter
	{
		void Attach(object waitType);
	}

	public interface ISingleWaiter<in TWaitType> : IWaiter where TWaitType : class
	{
		void Attach(TWaitType waitType);

		void IWaiter.Attach(object waitType)
		{
			if (waitType is TWaitType typed)
				Attach(typed);
		}
	}

	public interface IArrayWaiter<in TWaitType> : ISingleWaiter<TWaitType> where TWaitType : class
	{
		void Attach(IEnumerable<TWaitType> waitTypes);
		
		void IWaiter.Attach(object waitType)
		{
			if (waitType is TWaitType[] typed)
				Attach(typed);
			if(waitType is TWaitType type)
				Attach(type);
		}
	}
}