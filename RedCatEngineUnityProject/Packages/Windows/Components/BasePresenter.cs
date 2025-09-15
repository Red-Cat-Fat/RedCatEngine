using System;
using Infrastructure.Windows.Interfaces;

namespace Infrastructure.Windows.Components
{
	public abstract class BasePresenter<TView, TModel> : IPresenter
		where TView : IView
		where TModel : class, IModel
	{
		public event Action CloseEvent;

		protected readonly TView View;

		protected BasePresenter(TView view)
		{
			View = view;
		}

		public void Open(IModel model)
		{
			View.ClickCloseEvent += Close;
			DoOpen(model as TModel);
			View.Open();
		}

		public void Close()
		{
			View.ClickCloseEvent -= Close;
			DoClose();
			View.Close();
			CloseEvent?.Invoke();
		}

		protected abstract void DoClose();

		protected abstract void DoOpen(TModel model);
	}
}