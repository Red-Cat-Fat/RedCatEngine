using RedCatEngine.Windows.Interfaces;

namespace RedCatEngine.Windows.Factory
{
	public class WindowData : IWindowData
	{
		private readonly IModel _model;
		private readonly IView _view;
		private readonly IPresenter _presenter;

		public WindowData(
			IModel model,
			IView view,
			IPresenter presenter
		)
		{
			_model = model;
			_view = view;
			_presenter = presenter;

		}

		public void Open()
		{
			_presenter.Open();
		}

		public void Close()
		{
			_presenter.Close();
		}
	}
}