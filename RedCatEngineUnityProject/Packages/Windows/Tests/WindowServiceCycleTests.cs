using System;
using System.Collections.Generic;
using System.Reflection;
using Infrastructure.Windows.Components.Windows;
using Infrastructure.Windows.Interfaces;
using NUnit.Framework;
using RedCatEngine.CommonServices.Services.Logs;
using RedCatEngine.DependencyInjection.Specials;
using RedCatEngine.Windows.Services;
using RedCatEngine.Configs;
using UnityEngine;

namespace RedCatEngine.Windows.Tests
{
	public class WindowServiceCycleTests
	{
		private static readonly FieldInfo ConfigIdField = typeof(BaseConfig).GetField("_id", BindingFlags.Instance | BindingFlags.NonPublic);

		[Test]
		public void Open_WithCyclicParents_StopsRecursionAndLogsError()
		{
			var logService = new TestLogService();
			var windowContainer = new TestWindowContainer();
			var windowService = new WindowService(windowContainer, logService);

			var firstWindow = CreateWindowConfig("First", 1);
			var secondWindow = CreateWindowConfig("Second", 2);

			firstWindow.ParentForData = secondWindow;
			secondWindow.ParentForData = firstWindow;

			windowService.Open(firstWindow);

			Assert.AreEqual(1, firstWindow.WindowData.OpenCount);
			Assert.AreEqual(1, secondWindow.WindowData.OpenCount);
			Assert.That(logService.ErrorLogs.Count, Is.EqualTo(1));
			Assert.That(logService.ErrorLogs[0], Does.Contain("Detected cyclic parent window reference"));
		}

		[Test]
		public void Open_WithNonCyclicParent_OpensParentAndChildWithoutErrors()
		{
			var logService = new TestLogService();
			var windowContainer = new TestWindowContainer();
			var windowService = new WindowService(windowContainer, logService);

			var childWindow = CreateWindowConfig("Child", 3);
			var parentWindow = CreateWindowConfig("Parent", 4);

			childWindow.ParentForData = parentWindow;
			parentWindow.ParentForData = null;

			windowService.Open(childWindow);

			Assert.AreEqual(1, parentWindow.WindowData.OpenCount);
			Assert.AreEqual(1, childWindow.WindowData.OpenCount);
			Assert.That(logService.ErrorLogs, Is.Empty);
		}

		private static TestWindowConfig CreateWindowConfig(string windowName, int id)
		{
			var windowConfig = ScriptableObject.CreateInstance<TestWindowConfig>();
			windowConfig.name = windowName;
			ConfigIdField?.SetValue(windowConfig, id);
			return windowConfig;
		}

		private class TestWindowConfig : WindowConfig
		{
			public TestWindowData WindowData { get; } = new TestWindowData();

			public WindowConfig ParentForData
			{
				set => WindowData.Parent = value;
			}

			public override Type ModelType
				=> typeof(TestModel);

			public override IWindowData MakeWindowData(RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind.ICreator windowContainer, params object[] context)
			{
				WindowData.Config = this;
				return WindowData;
			}
		}

		private class TestWindowData : IWindowData
		{
			public WindowLayer Layer => WindowLayer.Screen;
			public bool IsOpen { get; private set; }
			public WindowConfig Config { get; set; }
			public WindowConfig Parent { get; set; }
			public int OpenCount { get; private set; }

			public void Open(IModel model)
			{
				OpenCount++;
				IsOpen = true;
			}

			public void Close()
			{
				IsOpen = false;
			}

			public void SetCloseCallback(Action onCloseCallBack)
			{
			}

			public bool TryGetParent(out WindowConfig parent)
			{
				parent = Parent;
				return parent != null;
			}
		}

		private class TestModel : IModel
		{
		}

		private class TestWindowContainer : IWindowContainer
		{
			public Injector Injector => null;

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

			public T GetSingle<T>(params object[] context) => default;
			public object GetSingle(Type type, params object[] context) => null;
			public IEnumerable<T> GetArray<T>() => Array.Empty<T>();
			public object Create(Type type, params object[] context) => Activator.CreateInstance(type);
			public T Create<T>(params object[] context) => Activator.CreateInstance<T>();

			public GameObject Create(GameObject prefab, Transform parent, params object[] context)
				=> throw new NotSupportedException();

			public GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, params object[] context)
				=> throw new NotSupportedException();

			public TBindType CreateAndGetComponent<TBindType>(GameObject prefab, Transform parent, params object[] context) where TBindType : Component
				=> throw new NotSupportedException();

			public TBindType CreateAndGetComponent<TBindType>(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, params object[] context) where TBindType : Component
				=> throw new NotSupportedException();

			public object CreateAndGetComponent(Type componentType, GameObject prefab, Transform parent, params object[] context)
				=> throw new NotSupportedException();

			public object CreateAndGetComponent(Type componentType, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, params object[] context)
				=> throw new NotSupportedException();

			public IModel FillContextToModel(IModel model, params object[] context)
				=> model;
		}

		private class TestLogService : ILogService
		{
			public List<string> ErrorLogs { get; } = new List<string>();

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
			{
				ErrorLogs.Add(log);
			}

			public void LogFormat(string log, params object[] parameters)
			{
			}

			public void LogWarningFormat(string log, params object[] parameters)
			{
			}

			public void LogErrorFormat(string log, params object[] parameters)
			{
				ErrorLogs.Add(string.Format(log, parameters));
			}
		}
	}
}
