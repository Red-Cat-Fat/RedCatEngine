using System;

namespace RedCatEngine.CommonServices.Services.Callbacks
{
	/// <summary>
	/// Обёртка над <see cref="Action"/>, которая позволяет вызвать действие один раз и/или при освобождении ресурсов.
	/// </summary>
	public class DisposableCallback : IDisposable
	{
		/// <summary>
		/// Действие, которое будет выполнено.
		/// </summary>
		private Action _callback;

		/// <summary>
		/// Флаг, указывающий, должно ли действие быть вызвано при освобождении (<see cref="Dispose"/>).
		/// </summary>
		private readonly bool _invokeOnDispose;

		/// <summary>
		/// Флаг, указывающий, было ли уже вызвано действие.
		/// </summary>
		private bool _isDisposed;

		/// <summary>
		/// Возвращает значение, указывающее, включён ли callback и может ли он быть вызван.
		/// </summary>
		public bool IsEnable { get; private set; }

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="DisposableCallback"/>.
		/// </summary>
		/// <param name="callback">Действие, которое будет выполнено.</param>
		/// <param name="invokeOnDispose"><see langword="true"/>, если действие должно быть вызвано при вызове <see cref="Dispose"/>.</param>
		public DisposableCallback(Action callback, bool invokeOnDispose)
		{
			_callback = callback;
			_invokeOnDispose = invokeOnDispose;
			IsEnable = true;
		}

		/// <summary>
		/// Вызывает сохранённое действие, если оно ещё не было вызвано.
		/// </summary>
		public void Invoke()
		{
			var callback = _callback;
			_callback = null;
			callback?.Invoke();
			IsEnable = false;
		}

		/// <summary>
		/// Освобождает ресурсы, связанные с этим объектом.
		/// Если <see cref="_invokeOnDispose"/> установлен в <see langword="true"/>, вызывает действие перед освобождением.
		/// </summary>
		public void Dispose()
		{
			if (_isDisposed)
				return;

			if (_invokeOnDispose)
			{
				Invoke();
			}
			else
			{
				IsEnable = false;
				_callback = null;
			}

			_isDisposed = true;
			DoDispose();
		}

		/// <summary>
		/// Метод для расширения логики освобождения ресурсов в производных классах.
		/// </summary>
		protected virtual void DoDispose()
		{
		}
	}
}