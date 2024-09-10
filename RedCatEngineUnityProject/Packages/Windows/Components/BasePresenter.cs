using RedCatEngine.Windows.Interfaces;

namespace RedCatEngine.Windows.Components
{
	public abstract class BasePresenter<TView, TModel> : IPresenter
		where TView : IView
		where TModel : IModel
	{
		private readonly TView View;
		protected readonly TModel Model;

		protected BasePresenter(TView view, TModel model)
		{
			View = view;
			Model = model;
		}

		public void Open()
		{
			View.CloseEvent += Close;
			DoOpen(View, Model);
			View.Open();
		}

		public void Close()
		{
			View.CloseEvent -= Close;
			DoClose(View, Model);
			View.Close();
		}

		protected abstract void DoClose(TView view, TModel model);

		protected abstract void DoOpen(TView view, TModel model);
	}
}