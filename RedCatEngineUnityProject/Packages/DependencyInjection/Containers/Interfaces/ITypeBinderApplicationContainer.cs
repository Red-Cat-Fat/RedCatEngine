namespace RedCatEngine.DependencyInjection.Containers.Interfaces
{
	public interface ITypeBinderApplicationContainer
	{
		TBindType BindType<TBindType, TInstanceType>()
			where TInstanceType : TBindType;
	}
}