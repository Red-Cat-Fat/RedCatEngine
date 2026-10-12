using System;
using Infrastructure.Windows.Components.Windows;

namespace Infrastructure.Windows.Interfaces
{
	public interface IWindowData
	{
		WindowLayer Layer { get; }
		bool IsOpen { get; }
		WindowConfig Config { get; }
		event Action ClosedEvent;
		void Open(IModel model);
		void Close();
		void Destroy();
		void SetCloseCallback(Action onCloseCallBack);
		bool TryGetParent(out WindowConfig parent);
	}
}
