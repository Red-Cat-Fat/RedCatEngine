namespace RedCatEngine.DependencyInjection.Specials.Providers
{
	public interface IProvider<TProvideType> where TProvideType : class
	{
		bool TryGet(out TProvideType instance);
	}
}