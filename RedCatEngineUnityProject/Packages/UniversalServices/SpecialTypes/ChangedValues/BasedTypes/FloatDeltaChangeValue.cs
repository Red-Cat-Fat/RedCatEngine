using System;

namespace RedCatEngine.CommonServices.SpecialTypes.ChangedValues.BasedTypes
{
	/// <summary>
/// Представляет изменение значения с плавающей точкой с возможностью вычисления дельты (разницы).
/// Является специализацией класса <see cref="DeltaChangeValue{T}"/> для типа <see cref="float"/>.
/// </summary>
public class FloatDeltaChangeValue : DeltaChangeValue<float>
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="FloatDeltaChangeValue"/>.
    /// </summary>
    /// <param name="oldValue">Старое значение.</param>
    /// <param name="newValue">Новое значение.</param>
    public FloatDeltaChangeValue(float oldValue, float newValue)
        : base(oldValue, newValue, (oldVal, newVal) => newVal - oldVal)
    {
    }

    /// <summary>
    /// Определяет неявное преобразование из кортежа со старым и новым значением в объект <see cref="FloatDeltaChangeValue"/>.
    /// </summary>
    /// <param name="tuple">Кортеж, содержащий <c>oldValue</c> и <c>newValue</c>.</param>
    /// <returns>Новый объект <see cref="FloatDeltaChangeValue"/>.</returns>
    public static implicit operator FloatDeltaChangeValue((float oldValue, float newValue) tuple)
        => new(tuple.oldValue, tuple.newValue);
}

}