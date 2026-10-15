using System;
using Infrastructure.Windows.Components.Windows;
using Infrastructure.Windows.Interfaces;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;

namespace RedCatEngine.Windows.Tests
{
	/// <summary>
	/// A window config for tests: no prefab, the window data only tracks open / closed state.
	/// </summary>
	public class CycleTestWindowConfig : WindowConfig
	{
		public override Type ModelType
			=> typeof(CycleTestModel);

		public override IWindowData MakeWindowData(ICreator windowContainer, params object[] context)
			=> new CycleTestWindowData(this);
	}

	public class CycleTestModel : IModel { }

	public class CycleTestWindowData : IWindowData
	{
		public CycleTestWindowData(WindowConfig config)
			=> Config = config;

		public event Action ClosedEvent;

		public WindowLayer Layer => WindowLayer.Screen;
		public bool IsOpen { get; private set; }
		public WindowConfig Config { get; }

		public void Open(IModel model)
			=> IsOpen = true;

		public void Close()
		{
			if (!IsOpen)
				return;

			IsOpen = false;
			ClosedEvent?.Invoke();
		}

		public void Destroy()
			=> Close();

		public void SetCloseCallback(Action onCloseCallBack) { }

		public bool TryGetParent(out WindowConfig parent)
		{
			parent = Config.Parent;
			return parent != null;
		}
	}
}
