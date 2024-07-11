using System.Collections.Generic;
using RedCatEngine.DependencyInjection.Containers.Interfaces;
using RedCatEngine.Windows.Components.Windows;
using RedCatEngine.Windows.Interfaces;

namespace RedCatEngine.Windows.Factory
{
	public class WindowFactory
	{
		private readonly ILayerContainer _layerContainer;
		private readonly IApplicationContainer _applicationContainer;
		private readonly Dictionary<uint, BaseWindowDataContainerConfig> _windows = new();

		public WindowFactory(ILayerContainer layerContainer, IApplicationContainer applicationContainer)
		{
			_layerContainer = layerContainer;
			_applicationContainer = applicationContainer;
		}

		public void Open<TModel>(IWindowSettings windowSettings) where TModel : IModel
		{
			if (_windows.TryGetValue(windowSettings.ID, out var window))
			{
				if(window.TryOpen(null, _applicationContainer))
					return;
			}
			var parent = _layerContainer.GetParentLayer(windowSettings.Layer);
		}
	}
}