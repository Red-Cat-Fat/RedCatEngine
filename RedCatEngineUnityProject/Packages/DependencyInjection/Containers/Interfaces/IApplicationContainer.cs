namespace RedCatEngine.DependencyInjection.Containers.Interfaces
{
	public interface IApplicationContainer :
		IBinderApplicationContainer,
		ITypeBinderApplicationContainer,
		IGetterApplicationContainer,
		IMonoConstructor,
		ICreator
	{
	}
}