using System;
using Infrastructure.Windows.Factory;
using Infrastructure.Windows.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;

namespace Infrastructure.Windows.Components.Windows
{
	public abstract class BaseModelWindowConfig<TModel, TView, TPresenter, TFactory> : WindowConfig
		where TModel : class, IModel
		where TView : BaseView
		where TPresenter : IPresenter
		where TFactory : WindowCreatorFactory<TView, TPresenter>
	{
		public override Type ModelType
			=> typeof(TModel);

		public override IWindowData MakeWindowData(ICreator windowContainer, params object[] context)
			=> windowContainer.Create<TFactory>(context).CreateWindow(this, context);
	}
}