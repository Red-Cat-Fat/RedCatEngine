using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Infrastructure.Windows.Components.Windows;
using Infrastructure.Windows.Interfaces;
using NUnit.Framework;
using RedCatEngine.CommonServices.Services.Logs;
using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;
using RedCatEngine.DependencyInjection.Specials;
using RedCatEngine.Windows.Services;
using UnityEngine;

namespace RedCatEngine.Windows.Tests
{
	public class WindowServiceTests
	{
		[Test]
		public void Open_WhenParentLinksCreateCycle_StopsRecursionAndLogsError()
		{
			var windowA = CreateWindowConfig(1, "WindowA");
			var windowB = CreateWindowConfig(2, "WindowB");
			SetParent(windowA, windowB);
			SetParent(windowB, windowA);

			var windowContainer = new TestWindowContainer();
			var logService = new TestLogService();
			var service = new WindowService(windowContainer, logService);

			service.Open(windowA);

			Assert.AreEqual(1, windowA.WindowData.OpenCalls, "Window A should be opened once");
			Assert.AreEqual(1, windowB.WindowData.OpenCalls, "Window B should be opened once");
			Assert.IsTrue(logService.Errors.Any(error => error.Contains("Detected cyclic parent chain")));
		}

		[Test]
		public void Open_WhenParentLinksAreAcyclic_OpensChildAndParentWithoutErrors()
		{
			var windowA = CreateWindowConfig(3, "WindowA");
			var windowB = CreateWindowConfig(4, "WindowB");
			SetParent(windowA, windowB);

			var windowContainer = new TestWindowContainer();
			var logService = new TestLogService();
			var service = new WindowService(windowContainer, logService);

			service.Open(windowA);

			Assert.AreEqual(1, windowA.WindowData.OpenCalls, "Window A should be opened once");
			Assert.AreEqual(1, windowB.WindowData.OpenCalls, "Window B should be opened once");
			Assert.IsEmpty(logService.Errors, "No errors are expected in acyclic case");
		}

		private static TestWindowConfig CreateWindowConfig(int id, string configName)
		{
			var config = ScriptableObject.CreateInstance<TestWindowConfig>();
			config.name = configName;
			SetBaseConfigId(config, id);
			return config;
		}

		private static void SetParent(WindowConfig child, WindowConfig parent)
			=> typeof(WindowConfig)
				.GetField("_parent", BindingFlags.Instance | BindingFlags.NonPublic)
				.SetValue(child, parent);

		private static void SetBaseConfigId(WindowConfig config, int id)
			=> typeof(BaseConfig)
				.GetField("_id", BindingFlags.Instance | BindingFlags.NonPublic)
				.SetValue(config, id);

		private class TestWindowConfig : WindowConfig
		{
			public TestWindowData WindowData { get; private set; }

			public override Type ModelType
				=> typeof(TestModel);

			public override IWindowData MakeWindowData(ICreator windowContainer, params object[] context)
			{
				WindowData ??= new TestWindowData(this, Parent, Layer);
				return WindowData;
			}
		}

		private class TestWindowData : IWindowData
		{
			private readonly WindowConfig _parent;

			public TestWindowData(WindowConfig config, WindowConfig parent, WindowLayer layer)
			{
				Config = config;
				_parent = parent;
				Layer = layer;
			}

			public int OpenCalls { get; private set; }

			public WindowLayer Layer { get; }
			public bool IsOpen { get; private set; }
			public WindowConfig Config { get; }

			public void Open(IModel model)
			{
				OpenCalls++;
				IsOpen = true;
			}

			public void Close()
				=> IsOpen = false;

			public void SetCloseCallback(Action onCloseCallBack)
			{
			}

			public bool TryGetParent(out WindowConfig parent)
			{
				parent = _parent;
				return parent != null;
			}
		}

		private class TestWindowContainer : IWindowContainer
		{
			public Injector Injector => null;

			public IModel FillContextToModel(IModel model, params object[] context)
				=> model;

			public object Create(Type type, params object[] context)
				=> new TestModel();

			public T Create<T>(params object[] context)
				=> (T)(object)new TestModel();

			public bool TryGetSingle<T>(out T data)
			{
				data = default;
				return false;
			}

			public bool TryGetSingle(Type type, out object data)
			{
				data = null;
				return false;
			}

			public bool TryGetArray<T>(out IEnumerable<T> data)
			{
				data = null;
				return false;
			}

			public T GetSingle<T>(params object[] context)
				=> default;

			public object GetSingle(Type type, params object[] context)
				=> null;

			public IEnumerable<T> GetArray<T>()
				=> Enumerable.Empty<T>();

			public GameObject Create(GameObject prefab, Transform parent, params object[] context)
				=> throw new NotSupportedException();

			public GameObject Create(
				GameObject prefab,
				Vector3 position,
				Quaternion rotation,
				Transform parent,
				params object[] context
			)
				=> throw new NotSupportedException();

			public TBindType CreateAndGetComponent<TBindType>(GameObject prefab, Transform parent, params object[] context) where TBindType : Component
				=> throw new NotSupportedException();

			public TBindType CreateAndGetComponent<TBindType>(
				GameObject prefab,
				Vector3 position,
				Quaternion rotation,
				Transform parent,
				params object[] context
			) where TBindType : Component
				=> throw new NotSupportedException();

			public object CreateAndGetComponent(Type componentType, GameObject prefab, Transform parent, params object[] context)
				=> throw new NotSupportedException();

			public object CreateAndGetComponent(
				Type componentType,
				GameObject prefab,
				Vector3 position,
				Quaternion rotation,
				Transform parent,
				params object[] context
			)
				=> throw new NotSupportedException();
		}

		private class TestModel : IModel
		{
		}

		private class TestLogService : ILogService
		{
			public readonly List<string> Errors = new List<string>();

			public ILogService CreateTag(string tag)
				=> this;

			public ILogService CreateTag<TType>()
				=> this;

			public void Log(string log)
			{
			}

			public void LogWarning(string log)
			{
			}

			public void LogError(string log)
				=> Errors.Add(log);

			public void LogFormat(string log, params object[] parameters)
			{
			}

			public void LogWarningFormat(string log, params object[] parameters)
			{
			}

			public void LogErrorFormat(string log, params object[] parameters)
				=> Errors.Add(string.Format(log, parameters));
		}
	}
}
