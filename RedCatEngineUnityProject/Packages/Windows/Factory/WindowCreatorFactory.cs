using JetBrains.Annotations;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.Windows.Components;
using RedCatEngine.Windows.Components.Windows;
using RedCatEngine.Windows.Interfaces;

namespace RedCatEngine.Windows.Factory
{
	public class WindowCreatorFactory<TModel, TView, TPresenter>
		where TModel : class, IModel
		where TView : BaseView
		where TPresenter : IPresenter
	{
		private readonly ILayerContainer _layerContainer;
		private readonly IWindowContainer _windowContainer;

		[Inject]
		[UsedImplicitly]
		public WindowCreatorFactory(ILayerContainer layerContainer, IWindowContainer windowContainer)
		{
			_layerContainer = layerContainer;
			_windowContainer = windowContainer;
		}

		public IWindowData CreateWindow(BaseWindowConfig config, params object[] context)
		{
			var parentTransform = _layerContainer.GetParentLayer(config.Layer);
			var model = config.GetModel() as TModel;
			var view = _windowContainer.CreateAndGetComponent<TView>(
				config.WindowPrefab,
				parentTransform,
				context: model);
			var presenter = _windowContainer.Create<TPresenter>(model, view, context);
			return new WindowData(
				model,
				view,
				presenter);
		}
	}
}