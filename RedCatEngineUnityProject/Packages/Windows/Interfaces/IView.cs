using System;

namespace RedCatEngine.Windows.Interfaces
{
	public interface IView
	{
		event Action CloseEvent;
		void Close();
		void Open();
	}
}