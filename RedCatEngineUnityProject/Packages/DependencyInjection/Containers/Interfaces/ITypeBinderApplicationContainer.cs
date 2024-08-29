namespace RedCatEngine.DependencyInjection.Containers.Interfaces
{
	public interface ITypeBinderApplicationContainer
	{
		TBindType BindType<TBindType, TInstanceType>(params object[] context)
			where TInstanceType : TBindType;

		TInstanceBindType BindType<TInstanceBindType>(params object[] context);
	}
}