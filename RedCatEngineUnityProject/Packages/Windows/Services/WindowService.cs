using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Windows.Components.Windows;
using Infrastructure.Windows.Interfaces;
using RedCatEngine.CommonServices.Extensions;
using RedCatEngine.CommonServices.Services.Logs;
using RedCatEngine.DependencyInjection.Containers.Attributes;

namespace RedCatEngine.Windows.Services
{
	public class WindowService : IWindowService
	{
		private readonly IWindowContainer _windowContainer;
		private readonly ILogService _logService;
		private readonly Dictionary<WindowConfig, IWindowData> _windowInfos;
		private readonly Dictionary<WindowConfig, List<WindowConfig>> _parentHierarchy;

		[Inject]
		public WindowService(IWindowContainer windowContainer, ILogService logService)
		{
			_windowContainer = windowContainer;
			_logService = logService.CreateTag<WindowService>();
			_windowInfos = new Dictionary<WindowConfig, IWindowData>();
			_parentHierarchy = new Dictionary<WindowConfig, List<WindowConfig>>();
		}

		public bool IsOpen(WindowConfig windowConfig)
			=> _windowInfos.TryGetValue(windowConfig, out var windowData) && windowData.IsOpen;

		public void Open(WindowConfig windowConfig, params object[] context)
		{
			_logService.LogFormat("Open window {0}", windowConfig.name);
			var windowData = PrepareWindowData(windowConfig, context);
			windowData.Open(
				(IModel)_windowContainer.Create(windowConfig.ModelType, CreateContext(windowConfig, context))
			);
		}

		public void OpenWithCallbacks(
			WindowConfig windowConfig,
			Action openCallback,
			Action closeCallBack,
			params object[] context
		)
		{
			_logService.LogFormat("Open window {0} with callback", windowConfig.name);
			var windowData = PrepareWindowData(windowConfig, context);

			openCallback?.Invoke();
			windowData.SetCloseCallback(closeCallBack);
			windowData.Open(
				(IModel)_windowContainer.Create(windowConfig.ModelType, CreateContext(windowConfig, context))
			);
		}

		private object[] CreateContext(WindowConfig windowConfig, object[] context)
			=> context.Attach(windowConfig, _logService);

		public void Close(WindowConfig windowConfig)
		{
			if (!_windowInfos.TryGetValue(windowConfig, out var info))
				return;

			_logService.LogFormat("Close window {0}", windowConfig.name);
			info.Close();
		}

		public void CloseAll()
		{
			_logService.Log("Close all windows");
			foreach (var windowInfo in _windowInfos)
				windowInfo.Value.Close();
		}

		private IWindowData PrepareWindowData(WindowConfig windowConfig, object[] context)
		{
			var windowData = GetWindowData(windowConfig, context);
			var isHasParent = TryOpenParent(
				windowConfig,
				context,
				windowData,
				out var parent
			);

			var openedLayer = windowData.Layer;
			IEnumerable<KeyValuePair<WindowConfig, IWindowData>> windowsForClose;

			if (isHasParent)
			{
				var parentHierarchy = GetSafeHierarchy(parent);
				windowsForClose = _windowInfos.Where(
					windowKeyValue
						=>
					{
						var tryCloseWindowConfig = windowKeyValue.Key;
						var tryCloseWindowData = windowKeyValue.Value;
						return tryCloseWindowConfig != windowConfig
							&& tryCloseWindowConfig != parent
							&& !parentHierarchy.Contains(tryCloseWindowConfig)
							&& tryCloseWindowData.Layer == openedLayer
							&& tryCloseWindowData.IsOpen;
					}
				);
			}
			else
			{
				if (_parentHierarchy.Keys.Contains(windowConfig))
				{
					windowsForClose = _windowInfos.Where(
						windowKeyValue
							=>
						{
							var tryCloseWindowData = windowKeyValue.Value;
							return tryCloseWindowData.Config != windowConfig
								&& tryCloseWindowData.Layer == openedLayer
								&& tryCloseWindowData.IsOpen
								&& !_parentHierarchy[windowConfig].Contains(windowKeyValue.Key);
						}
					);
				}
				else
				{
					windowsForClose = _windowInfos.Where(
						windowKeyValue
							=>
						{
							var tryCloseWindowData = windowKeyValue.Value;
							return tryCloseWindowData.Config != windowConfig
								&& tryCloseWindowData.Layer == openedLayer
								&& tryCloseWindowData.IsOpen
								&& windowKeyValue.Key != windowConfig;
						}
					).ToArray();
				}
			}

			var sb = new System.Text.StringBuilder();
			sb.AppendLine(
				string.Format("{1} Close {0} windows:",
				windowsForClose.Count(),
				windowConfig.name));
			foreach (var closeWindow in windowsForClose)
			{
				sb.AppendLine("\t" + closeWindow.Key.name);
			}
			_logService.Log(sb.ToString());

			foreach (var windowKeyValue in windowsForClose)
				windowKeyValue.Value.Close();

			return windowData;
		}

		private bool TryOpenParent(
			WindowConfig windowConfig,
			object[] context,
			IWindowData windowData,
			out WindowConfig parentWindowConfig
		)
		{
			var isHasParent = windowData.TryGetParent(out parentWindowConfig);
			if (!isHasParent)
				return false;

			var value = GetSafeHierarchy(parentWindowConfig);
			if (!value.Contains(windowConfig))
			{
				value.Add(windowConfig);
			}

			Open(parentWindowConfig, context);

			return true;
		}

		private List<WindowConfig> GetSafeHierarchy(WindowConfig parentWindowConfig)
		{
			if (_parentHierarchy.TryGetValue(parentWindowConfig, out var value))
				return value;

			value = new List<WindowConfig>();
			_parentHierarchy[parentWindowConfig] = value;
			return value;
		}

		private IWindowData GetWindowData(WindowConfig windowConfig, object[] context)
		{
			IWindowData windowData;
			if (_windowInfos.TryGetValue(windowConfig, out var info))
			{
				windowData = info;
			}
			else
			{
				windowData = windowConfig.MakeWindowData(_windowContainer, CreateContext(windowConfig, context));
				_windowInfos.Add(windowConfig, windowData);
			}

			return windowData;
		}
	}
}
