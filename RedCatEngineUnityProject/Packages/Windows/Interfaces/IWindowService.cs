using System;
using Infrastructure.Windows.Components.Windows;

namespace Infrastructure.Windows.Interfaces
{
	public interface IWindowService
	{
		event Action<WindowConfig> WindowOpenedEvent;
		event Action<WindowConfig> WindowClosedEvent;

		bool IsOpen(WindowConfig windowConfig);
		void Open(WindowConfig windowConfig, params object[] context);

		void OpenWithCallbacks(
			WindowConfig windowConfig,
			Action openCallback,
			Action closeCallBack,
			params object[] context
		);

		void Close(WindowConfig windowConfig);

		void HideAll();
		void ShowAll();
	}
}
