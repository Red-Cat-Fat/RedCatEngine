using System;
using RedCatEngine.CommonServices.Services.Times.TimeServices;
using RedCatEngine.DependencyInjection.Containers.Attributes;

namespace RedCatEngine.CommonServices.Services.Callbacks
{
	/// <summary>
	/// Обёртка над <see cref="DisposableCallback"/>, которая вызывает действие после истечения заданного времени.
	/// Использует <see cref="ITimeService"/> для отсчёта времени.
	/// </summary>
	public class DisposableTimerCallback : DisposableCallback
	{
		/// <summary>
		/// Служба времени, используемая для отсчёта таймера.
		/// </summary>
		private readonly ITimeService _timeService;

		/// <summary>
		/// Общее время таймера в секундах.
		/// </summary>
		private readonly float _timer;
		public float TimeLeft
			=> _timer - _time;

		/// <summary>
		/// Текущее прошедшее время с начала таймера.
		/// </summary>
		private float _time;

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="DisposableTimerCallback"/>.
		/// </summary>
		/// <param name="timer">Длительность таймера в секундах.</param>
		/// <param name="callback">Действие, которое будет вызвано по истечении таймера.</param>
		/// <param name="invokeOnDispose"><see langword="true"/>, если действие должно быть вызвано при вызове <see cref="Dispose"/>.</param>
		[Inject]
		public DisposableTimerCallback(
			float timer,
			Action callback,
			bool invokeOnDispose
		)
			: base(callback, invokeOnDispose)
		{
			_timer = timer;
		}

		/// <summary>
		/// Вызывается каждый кадр для обновления таймера.
		/// Если время истекло, вызывается <see cref="Invoke"/> и объект уничтожается.
		/// </summary>
		/// <param name="deltaTime">Время, прошедшее с последнего кадра.</param>
		public void OnUpdate(float deltaTime)
		{
			_time += deltaTime;
			if (_time < _timer)
				return;
			Invoke();
			Dispose();
		}

		/// <summary>
		/// Освобождает ресурсы и завершает таймер.
		/// Устанавливает прошедшее время равным общему времени таймера.
		/// </summary>
		protected override void DoDispose()
		{
			_time = _timer;
		}
	}
}