using System;
using Infrastructure.Windows.Components.Windows;
using Infrastructure.Windows.Interfaces;
using UnityEngine;

namespace Infrastructure.Windows.Factory
{
	public class WindowData : IWindowData
	{
		private readonly WindowConfig _parentConfig;

		private readonly IPresenter _presenter;
		private readonly IView _view;
		private bool _isDestroyed;
		private Action _onCloseCallBack;

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
		}

		public event Action ClosedEvent;

		public WindowLayer Layer { get; }

		public WindowConfig Config { get; }

		public bool IsOpen
		{
			get
			{
				if(_isDestroyed)
					return false;

				if(_view is UnityEngine.Object unityView && unityView == null)
					return false;

				return _view.IsOpen;
			}
		}

		public bool TryGetParent(out WindowConfig parent)
		{
			parent = _parentConfig;
			return parent != null;
		}

		public void Open(IModel model)
		{
			if(_isDestroyed || IsOpen)
				return;

			_presenter.CloseEvent += OnPresenterClosed;
			_presenter.Open(model);
		}

		public void Close()
		{
			if(_isDestroyed || !IsOpen)
				return;

			_presenter.Close();
		}

		public void Destroy()
		{
			if(_isDestroyed)
				return;

			if(IsOpen)
				Close();

			_isDestroyed = true;
			DestroyViewObject();
		}

		public void SetCloseCallback(Action onCloseCallBack)
			=> _onCloseCallBack = onCloseCallBack;

		public void InjectContext(object[] context)
		{
		}

		private void OnPresenterClosed()
		{
			_presenter.CloseEvent -= OnPresenterClosed;
			var closeCallBack = _onCloseCallBack;
			_onCloseCallBack = null;
			closeCallBack?.Invoke();
			ClosedEvent?.Invoke();
		}

		private void DestroyViewObject()
		{
			if(_view is not Component component)
				return;

			if(component == null)
				return;

			var viewObject = component.gameObject;
			if(viewObject == null)
				return;

			if(Application.isPlaying)
				UnityEngine.Object.Destroy(viewObject);
			else
				UnityEngine.Object.DestroyImmediate(viewObject);
		}
	}
}
