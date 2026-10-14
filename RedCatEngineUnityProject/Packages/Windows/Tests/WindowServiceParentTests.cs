using System.Reflection;
using Infrastructure.Windows.Components.Windows;
using Infrastructure.Windows.Services;
using NUnit.Framework;
using RedCatEngine.CommonServices.Services.Logs;
using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Specials.CreatedRule;
using UnityEngine;

namespace RedCatEngine.Windows.Tests
{
	public class WindowServiceParentTests
	{
		private uint _nextId = 1;
		private UnityGameContainer _container;
		private WindowService _windows;

		[SetUp]
		public void SetUp()
		{
			_container = new UnityGameContainer(new UnityContainerModifyRule());
			_windows = new WindowService(_container, NullLogService.Instance);
		}

		[TearDown]
		public void TearDown()
		{
			_windows.Dispose();
			_container.Dispose();
		}

		private CycleTestWindowConfig MakeConfig(string name)
		{
			var config = ScriptableObject.CreateInstance<CycleTestWindowConfig>();
			config.name = name;
			// configs are compared by Id: every test config needs its own
			typeof(BaseConfig)
				.GetField("_id", BindingFlags.Instance | BindingFlags.NonPublic)
				.SetValue(config, _nextId++);
			return config;
		}

		private static void SetParent(WindowConfig config, WindowConfig parent)
			=> typeof(WindowConfig)
				.GetField("_parent", BindingFlags.Instance | BindingFlags.NonPublic)
				.SetValue(config, parent);

		[Test]
		public void GivenChildWithClosedParent_WhenOpenChild_ThenParentOpened()
		{
			var parent = MakeConfig("Parent");
			var child = MakeConfig("Child");
			SetParent(child, parent);

			_windows.Open(child);

			Assert.IsTrue(_windows.IsOpen(child));
			Assert.IsTrue(_windows.IsOpen(parent));
		}

		[Test]
		public void GivenCyclicParents_WhenOpen_ThenOpeningStopsAndBothOpen()
		{
			var first = MakeConfig("First");
			var second = MakeConfig("Second");
			SetParent(first, second);
			SetParent(second, first);

			Assert.DoesNotThrow(() => _windows.Open(first));
			Assert.IsTrue(_windows.IsOpen(first));
			Assert.IsTrue(_windows.IsOpen(second));
		}

		[Test]
		public void GivenWindowThatIsItsOwnParent_WhenOpen_ThenOpened()
		{
			var window = MakeConfig("Self");
			SetParent(window, window);

			Assert.DoesNotThrow(() => _windows.Open(window));
			Assert.IsTrue(_windows.IsOpen(window));
		}
	}
}
