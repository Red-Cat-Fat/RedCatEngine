using System;
using Infrastructure.Windows.Components.Windows;
using Infrastructure.Windows.Interfaces;

namespace Infrastructure.Windows.Factory
{
	public class WindowData : IWindowData
	{
		private readonly IPresenter _presenter;
		private readonly WindowConfig _parentConfig;
		private readonly IView _view;
		private Action _onCloseCallBack;

		public WindowLayer Layer { get; }

		public WindowConfig Config { get; }

		public bool IsOpen
			=> _view.IsOpen;

		public WindowData(
			WindowConfig currentConfig,
			WindowConfig parentConfig,
			WindowLayer layer,
			IView view,
			IPresenter presenter
		)
		{
			Layer = layer;
			Config = currentConfig;
			_parentConfig = parentConfig;
			_view = view;
			_presenter = presenter;

			_presenter.CloseEvent += ClosePresenterTrigger;
		}

		public bool TryGetParent(out WindowConfig parent)
		{
			parent = _parentConfig;
			return parent != null;
		}

		public void Open(IModel model)
		{
			if (IsOpen)
				return;
			_presenter.Open(model);
		}

		public void Close()
		{
			if (!IsOpen)
				return;
			_presenter.Close();
		}

		public void SetCloseCallback(Action onCloseCallBack)
			=> _onCloseCallBack = onCloseCallBack;

		public void InjectContext(object[] context)
		{
		}

		private void ClosePresenterTrigger()
		{
			_presenter.CloseEvent -= ClosePresenterTrigger;
			_onCloseCallBack?.Invoke();
			_onCloseCallBack = null;
		}
	}
}