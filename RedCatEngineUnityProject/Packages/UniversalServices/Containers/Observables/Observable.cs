using System;
using System.Collections.Generic;

namespace RedCatEngine.CommonServices.Containers.Observables
{
	public class Observable<T>
	{
		private T _value;
		public event Action<T> ValueChangeEvent;

		public T Value
		{
			get => _value;
			set => Set(value);
		}

		public static implicit operator T(Observable<T> observable)
			=> observable._value;

		public Observable(T value, Action<T> onValueChanged = null)
		{
			this._value = value;

			if (onValueChanged != null)
				ValueChangeEvent += onValueChanged;
		}

		public void Set(T value)
		{
			if (EqualityComparer<T>.Default.Equals(this._value, value))
				return;
			this._value = value;
			Invoke();
		}

		private void Invoke()
		{
			ValueChangeEvent?.Invoke(_value);
		}

		public void AddListener(Action<T> handler)
		{
			ValueChangeEvent += handler;
		}

		public void RemoveListener(Action<T> handler)
		{
			ValueChangeEvent -= handler;
		}

		public void Dispose()
		{
			ValueChangeEvent = null;
			_value = default;
		}
	}
}