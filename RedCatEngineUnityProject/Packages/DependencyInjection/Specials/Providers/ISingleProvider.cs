namespace RedCatEngine.DependencyInjection.Specials.Providers
{
	public interface ISingleProvider<TProvideType> where TProvideType : class
	{
		bool TryGet(out TProvideType instance);
	}
}