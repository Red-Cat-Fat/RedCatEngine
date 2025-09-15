namespace RedCatEngine.CommonServices.SpecialTypes.ChangedValues.BasedTypes
{
	/// <summary>
/// Представляет изменение целочисленного значения с возможностью вычисления дельты (разницы).
/// Является специализацией класса <see cref="DeltaChangeValue{T}"/> для типа <see cref="int"/>.
/// </summary>
public class IntDeltaChangeValue : DeltaChangeValue<int>
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="IntDeltaChangeValue"/>.
    /// </summary>
    /// <param name="oldValue">Старое значение.</param>
    /// <param name="newValue">Новое значение.</param>
    public IntDeltaChangeValue(int oldValue, int newValue)
        : base(oldValue, newValue, (oldVal, newVal) => newVal - oldVal)
    {
    }

    /// <summary>
    /// Определяет неявное преобразование из кортежа со старым и новым значением в объект <see cref="IntDeltaChangeValue"/>.
    /// </summary>
    /// <param name="tuple">Кортеж, содержащий <c>oldValue</c> и <c>newValue</c>.</param>
    /// <returns>Новый объект <see cref="IntDeltaChangeValue"/>.</returns>
    public static implicit operator IntDeltaChangeValue((int oldValue, int newValue) tuple)
        => new(tuple.oldValue, tuple.newValue);
}

}