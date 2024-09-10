using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;
using RedCatEngine.Windows.Factory;
using RedCatEngine.Windows.Interfaces;

namespace RedCatEngine.Windows.Components.Windows
{
	public abstract class BaseModelWindowConfig<TModel, TView, TPresenter, TFactory> : BaseWindowConfig
		where TModel : class, IModel
		where TView : BaseView
		where TPresenter : IPresenter
		where TFactory : WindowCreatorFactory<TModel, TView, TPresenter>
	{
		protected abstract TModel MakeModel();

		public override IModel GetModel()
			=> MakeModel();

		public override IWindowData MakeWindowData(ICreator windowContainer, params object[] context)
			=> windowContainer.Create<TFactory>(context).CreateWindow(this, context);
	}
}