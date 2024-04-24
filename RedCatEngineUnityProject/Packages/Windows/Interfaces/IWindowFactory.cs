using RedCatEngine.Windows.Components.Windows;

namespace RedCatEngine.Windows.Interfaces
{
	public interface IWindowFactory<TModel> where TModel : IModel
	{
		bool Make(TModel model);
	}

	public interface IWindowFactory
	{
		bool Open(BaseWindowDataContainerConfig windowDataContainerConfig);
	}
}