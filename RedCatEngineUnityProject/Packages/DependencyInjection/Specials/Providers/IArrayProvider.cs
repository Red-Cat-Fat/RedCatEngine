namespace RedCatEngine.DependencyInjection.Specials.Providers
{
	public interface IArrayProvider<TProvideType> where TProvideType : class
	{
		bool TryGet(out TProvideType[] instance);
	}
}