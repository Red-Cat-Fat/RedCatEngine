namespace RedCatEngine.DependencyInjection.Containers.Interfaces
{
	public interface ITypeBinderApplicationContainer
	{
		TInstanceBindType BindDummy<TInstanceBindType, TDummyType>(params object[] context)
			where TDummyType : TInstanceBindType;

		TBindType BindType<TBindType, TInstanceType>(params object[] context)
			where TInstanceType : TBindType;

		TInstanceBindType BindType<TInstanceBindType>(params object[] context);
	}
}