using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Windows.Components.Windows;
using Infrastructure.Windows.Interfaces;
using RedCatEngine.CommonServices.Extensions;
using RedCatEngine.CommonServices.Services.Logs;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;

namespace Infrastructure.Windows.Services
{
	public class WindowService : IWindowService, IDisposable
	{
		private readonly List<WindowConfig> _hiddenWindows = new();
		private readonly ILogService _logService;
		private readonly Dictionary<WindowConfig, List<WindowConfig>> _parentHierarchy;

		private readonly IUnityGameContainer _windowContainer;
		private readonly Dictionary<WindowConfig, IWindowData> _windowInfos;
		private bool _isDisposed;

		[Inject]
		public WindowService(IUnityGameContainer windowContainer, ILogService logService)
		{
			_windowContainer = windowContainer;
			_logService = logService.CreateTag<WindowService>();
			_windowInfos = new Dictionary<WindowConfig, IWindowData>();
			_parentHierarchy = new Dictionary<WindowConfig, List<WindowConfig>>();
		}

		public void Dispose()
		{
			if(_isDisposed)
				return;

			_isDisposed = true;
			_logService.Info("Dispose: destroy {0} windows", _windowInfos.Count);

			try
			{
				foreach(var windowData in _windowInfos.Values.ToArray())
				{
					try
					{
						windowData.Destroy();
					}
					catch(Exception exception)
					{
						_logService.WriteException(exception, "WindowService.Dispose");
					}
				}
			}
			finally
			{
				_windowInfos.Clear();
				_hiddenWindows.Clear();
				_parentHierarchy.Clear();
			}
		}

		public event Action<WindowConfig> WindowOpenedEvent;
		public event Action<WindowConfig> WindowClosedEvent;

		public bool IsOpen(WindowConfig windowConfig)
			=> !_isDisposed
			   && _windowInfos.TryGetValue(windowConfig, out var windowData)
			   && windowData.IsOpen;

		public void Open(WindowConfig windowConfig, params object[] context)
		{
			if(_isDisposed)
				return;

			_logService.Info("Open window {0}", windowConfig.name);
			var windowData = PrepareWindowData(windowConfig, context);
			windowData.Open(
					(IModel)_windowContainer.Create(windowConfig.ModelType, CreateContext(windowConfig, context))
			);
			NotifyWindowOpened(windowConfig, windowData);
		}

		public void OpenWithCallbacks(
				WindowConfig windowConfig,
				Action openCallback,
				Action closeCallBack,
				params object[] context
		)
		{
			if(_isDisposed)
				return;

			_logService.Info("Open window {0} with callback", windowConfig.name);
			var windowData = PrepareWindowData(windowConfig, context);

			openCallback?.Invoke();
			windowData.SetCloseCallback(closeCallBack);
			windowData.Open(
					(IModel)_windowContainer.Create(windowConfig.ModelType, CreateContext(windowConfig, context))
			);
			NotifyWindowOpened(windowConfig, windowData);
		}

		public void Close(WindowConfig windowConfig)
		{
			if(_isDisposed)
				return;

			if(!_windowInfos.TryGetValue(windowConfig, out var info) || !info.IsOpen)
				return;

			_logService.Info("Close window {0}", windowConfig);
			info.Close();
		}

		public void HideAll()
		{
			if(_isDisposed)
				return;

			_hiddenWindows.Clear();
			foreach(var kvp in _windowInfos)
			{
				if(kvp.Value.IsOpen)
				{
					_hiddenWindows.Add(kvp.Key);
					kvp.Value.Close();
				}
			}

			_logService.Info("HideAll: saved {0} windows", _hiddenWindows.Count);
		}

		public void ShowAll()
		{
			if(_isDisposed)
				return;

			foreach(var config in _hiddenWindows)
			{
				Open(config);
			}

			_hiddenWindows.Clear();
			_logService.Info("ShowAll: restored windows");
		}

		private object[] CreateContext(WindowConfig windowConfig, object[] context)
			=> context.Attach(windowConfig, _logService);

		private void NotifyWindowOpened(WindowConfig windowConfig, IWindowData windowData)
		{
			if(!windowData.IsOpen)
				return;

			WindowOpenedEvent?.Invoke(windowConfig);
		}

		private void NotifyWindowClosed(WindowConfig windowConfig)
			=> WindowClosedEvent?.Invoke(windowConfig);

		public void CloseAll()
		{
			if(_isDisposed)
				return;

			_logService.Info("Close all windows");
			foreach(var windowInfo in _windowInfos)
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

			if(isHasParent)
			{
				var parentHierarchy = GetSafeHierarchy(parent);
				windowsForClose = _windowInfos.Where(windowKeyValue
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
				if(_parentHierarchy.Keys.Contains(windowConfig))
				{
					windowsForClose = _windowInfos.Where(windowKeyValue
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
					windowsForClose = _windowInfos.Where(windowKeyValue
									=>
							{
								var tryCloseWindowData = windowKeyValue.Value;
								return tryCloseWindowData.Config != windowConfig
								       && tryCloseWindowData.Layer == openedLayer
								       && tryCloseWindowData.IsOpen
								       && windowKeyValue.Key != windowConfig;
							}
					);
				}
			}

			var countCloseWindows = windowsForClose.Count();
			if(countCloseWindows > 0)
			{
				var sb = new System.Text.StringBuilder();
				sb.AppendFormat(
						"{0} Close {1} windows:",
						windowConfig.name,
						countCloseWindows
				).AppendLine();
				foreach(var closeWindow in windowsForClose)
				{
					sb.AppendLine("\t" + closeWindow.Key.name);
				}

				_logService.Info(sb.ToString());
			}
			else
			{
				_logService.Info(
						"{0} not close other windows.",
						windowConfig.name
				);
			}

			foreach(var windowKeyValue in windowsForClose)
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
			if(!isHasParent)
				return false;

			var value = GetSafeHierarchy(parentWindowConfig);
			if(!value.Contains(windowConfig))
			{
				value.Add(windowConfig);
			}

			if(!IsOpen(parentWindowConfig))
			{
				Open(parentWindowConfig, context);
			}

			return true;
		}

		private List<WindowConfig> GetSafeHierarchy(WindowConfig parentWindowConfig)
		{
			if(_parentHierarchy.TryGetValue(parentWindowConfig, out var value))
				return value;

			value = new List<WindowConfig>();
			_parentHierarchy[parentWindowConfig] = value;
			return value;
		}

		private IWindowData GetWindowData(WindowConfig windowConfig, object[] context)
		{
			IWindowData windowData;
			if(_windowInfos.TryGetValue(windowConfig, out var info))
			{
				info.Close();
				windowData = info;
			}
			else
			{
				windowData = windowConfig.MakeWindowData(_windowContainer, CreateContext(windowConfig, context));
				windowData.ClosedEvent += () => NotifyWindowClosed(windowConfig);
				_windowInfos[windowConfig] = windowData;
			}


			return windowData;
		}
	}
}
