using System;

namespace Infrastructure.Windows.Interfaces
{
	public interface IPresenter
	{
		event Action CloseEvent;
		void Open(IModel model);
		void Close();
	}
}