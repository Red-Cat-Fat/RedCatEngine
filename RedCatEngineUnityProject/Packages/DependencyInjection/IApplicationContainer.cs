namespace RedCatEngine.DependencyInjection
{
	public interface IApplicationContainer
	{
		bool TryGet<T>(out T data);
	}
}