using System;

namespace Infrastructure.Windows.Interfaces
{
	public interface IView
	{
		bool IsOpen { get; }
		event Action ClickCloseEvent;
		void Close();
		void Open();
	}
}