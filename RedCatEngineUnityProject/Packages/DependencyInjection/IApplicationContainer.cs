namespace RedCatEngine.DependencyInjection
{
	public interface IApplicationContainer
	{
		void Bind<T>(T instance);
		bool TryGet<T>(out T data);
		T Get<T>();
	}
}