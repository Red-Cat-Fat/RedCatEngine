using System.Collections.Generic;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.Windows.Components.Windows;
using RedCatEngine.Windows.Interfaces;

namespace RedCatEngine.Windows.Services
{
	public class WindowService : IWindowService
	{
		private readonly IWindowContainer _windowContainer;
		private readonly Dictionary<int, IWindowData> _windowInfos;

		[Inject]
		public WindowService(IWindowContainer windowContainer)
		{
			_windowContainer = windowContainer;
			_windowInfos = new Dictionary<int, IWindowData>();
		}

		public void Open(BaseWindowConfig windowConfig, params object[] context)
		{
			IWindowData windowData;
			if (_windowInfos.ContainsKey(windowConfig.ID))
			{
				windowData = _windowInfos[windowConfig.ID];
			}
			else
			{
				windowData = windowConfig.MakeWindowData(_windowContainer, context);
				_windowInfos.Add(windowConfig.ID, windowData);
			}
			windowData.Open();
		}
	}
}