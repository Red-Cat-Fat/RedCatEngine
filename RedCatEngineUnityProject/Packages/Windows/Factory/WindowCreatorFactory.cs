using Infrastructure.Windows.Components;
using Infrastructure.Windows.Components.Windows;
using Infrastructure.Windows.Interfaces;
using JetBrains.Annotations;
using RedCatEngine.CommonServices.Extensions;
using RedCatEngine.CommonServices.Services.Logs;
using RedCatEngine.DependencyInjection.Containers.Attributes;

namespace Infrastructure.Windows.Factory
{
	public class WindowCreatorFactory<TView, TPresenter>
		where TView : BaseView
		where TPresenter : IPresenter
	{
		private readonly ILayerContainer _layerContainer;
		private readonly IWindowContainer _windowContainer;
		private readonly ILogService _log;

		[Inject]
		public WindowCreatorFactory(ILayerContainer layerContainer, IWindowContainer windowContainer, ILogService log)
		{
			_layerContainer = layerContainer;
			_windowContainer = windowContainer;
			_log = log;
		}

		public IWindowData CreateWindow(WindowConfig config, params object[] context)
		{
			var parentTransform = _layerContainer.GetParentLayer(config.Layer);
			var fullContext =
				context.Attach(_log);
			var view = _windowContainer.CreateAndGetComponent<TView>(
				config.WindowPrefab,
				parentTransform,
				context: fullContext
			);
			var presenter = _windowContainer.Create<TPresenter>(
				view,
				fullContext
			);
			return new WindowData(
				config,
				config.Parent,
				config.Layer,
				view,
				presenter
			);
		}
	}
}