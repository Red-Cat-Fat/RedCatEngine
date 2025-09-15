namespace RedCatEngine.CommonServices.SpecialTypes.ChangeTrackedValues
{
	/// <summary>
	/// Обёртка для значения, которое информирует о том,
	/// было ли оно изменено в результате попытки установить новое значение.
	/// </summary>
	/// <typeparam name="TValue">Тип хранимого значения.</typeparam>
	public class ChangeTrackedValue<TValue>
	{
		/// <summary>
		/// Хранит текущее значение.
		/// </summary>
		private TValue _value;

		/// <summary>
		/// Получает текущее значение. Не позволяет его изменить напрямую.
		/// </summary>
		public TValue Value => _value;

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ChangeTrackedValue{TValue}"/> со значением по умолчанию.
		/// </summary>
		public ChangeTrackedValue()
		{
			_value = default;
		}

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="ChangeTrackedValue{TValue}"/> с указанным начальным значением.
		/// </summary>
		/// <param name="value">Начальное значение.</param>
		public ChangeTrackedValue(TValue value)
		{
			_value = value;
		}

		/// <summary>
		/// Пытается задать новое значение. Возвращает результат того, было ли значение изменено или нет.
		/// </summary>
		/// <param name="value">Новое значение.</param>
		/// <returns>
		/// <see langword="true"/>, если новое значение было успешно изменено,
		/// <see langword="false"/>, если значение уже соответствует текущему и оно не было изменено.
		/// </returns>
		public bool TrySetNewValue(TValue value)
		{
			if (Equals(_value, value))
				return false;

			_value = value;
			return true;
		}

		/// <summary>
		/// Определяет неявное преобразование из значения типа <typeparamref name="TValue"/>
		/// в экземпляр класса <see cref="ChangeTrackedValue{TValue}"/>.
		/// </summary>
		/// <param name="value">Значение для обёртки.</param>
		/// <returns>Новый экземпляр класса <see cref="ChangeTrackedValue{TValue}"/>.</returns>
		public static implicit operator ChangeTrackedValue<TValue>(TValue value)
			=> new(value);

		/// <summary>
		/// Определяет неявное преобразование из экземпляра класса <see cref="ChangeTrackedValue{TValue}"/>
		/// в значение <typeparamref name="TValue"/>.
		/// </summary>
		/// <param name="value">Значение для обёртки.</param>
		/// <returns>Значение типа <typeparamref name="TValue"/>.</returns>
		public static implicit operator TValue(ChangeTrackedValue<TValue> value) => value.Value;
	}
}