using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using RedCatEngine.CommonServices.Services.DisposableCallbacks;
using RedCatEngine.CommonServices.Services.Times.TimeServices;

namespace RedCatEngine.UniversalServices.Tests
{
	public class DisposableCallbackServiceTests
	{
		private class ManualTimeService : ITimeService
		{
			public float FixedDeltaTime => 0f;
			public float DeltaTime => 0f;
			public float TimeScale => 1f;
			public float TotalTime { get; private set; }
			public event Action<float> UpdateEvent;

			public void Tick(float deltaTime)
			{
				TotalTime += deltaTime;
				UpdateEvent?.Invoke(deltaTime);
			}
		}

		private ManualTimeService _time;
		private DisposableCallbackService _service;

		[SetUp]
		public void SetUp()
		{
			_time = new ManualTimeService();
			_service = new DisposableCallbackService(_time);
		}

		[TearDown]
		public void TearDown()
			=> _service.Dispose();

		private int TrackedCallbacksCount
			=> ((ICollection)typeof(DisposableCallbackService)
				.GetField("_callbacks", BindingFlags.Instance | BindingFlags.NonPublic)
				.GetValue(_service)).Count;

		[Test]
		public void GivenInvokedCallbacks_WhenNewCallbackCreated_ThenFinishedCallbacksReleased()
		{
			for (var i = 0; i < 100; i++)
				_service.MakeCallback(() => { }).Invoke();
			for (var i = 0; i < 100; i++)
				_service.MakeCallback(() => { }).Dispose();

			var pending = _service.MakeCallback(() => { });

			Assert.AreEqual(1, TrackedCallbacksCount, "Invoked and disposed callbacks must not be kept");
			Assert.IsTrue(pending.IsEnable);
		}

		[Test]
		public void GivenAlwaysInvokeCallback_WhenServiceDisposed_ThenCallbackInvokedOnce()
		{
			var calls = 0;
			_service.MakeAlwaysInvokeCallback(() => calls++);

			_service.Dispose();
			_service.Dispose();

			Assert.AreEqual(1, calls);
		}

		[Test]
		public void GivenAlwaysInvokeCallbackCreatingCallbackOnDispose_WhenServiceDisposed_ThenNoException()
		{
			_service.MakeAlwaysInvokeCallback(() => _service.MakeCallback(() => { }));

			Assert.DoesNotThrow(() => _service.Dispose());
		}

		[Test]
		public void GivenTimer_WhenTimeElapsed_ThenCallbackInvokedOnce()
		{
			var calls = 0;
			_service.MakeTimerCallback(1f, () => calls++);

			_time.Tick(0.5f);
			Assert.AreEqual(0, calls);

			_time.Tick(0.6f);
			_time.Tick(1f);
			Assert.AreEqual(1, calls);
		}

		[Test]
		public void GivenCancelledTimer_WhenTimeElapsed_ThenCallbackNotInvoked()
		{
			var calls = 0;
			var timer = _service.MakeTimerCallback(1f, () => calls++);

			timer.Dispose();
			_time.Tick(2f);

			Assert.AreEqual(0, calls);
		}
	}
}
