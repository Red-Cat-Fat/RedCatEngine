using System;

namespace RedCatEngine.CommonServices.SpecialTypes.ChangedValues
{
	/// <summary>
/// Расширяет <see cref="ChangeValue{T}"/>, добавляя возможность вычисления разницы (дельты) между старым и новым значением.
/// </summary>
/// <typeparam name="T">Тип значения, для которого рассчитывается дельта.</typeparam>
public class DeltaChangeValue<T> : ChangeValue<T>
{
    /// <summary>
    /// Функция, используемая для вычисления дельты между старым и новым значением.
    /// </summary>
    private readonly Func<T, T, T> _deltaCalculator;

    /// <summary>
    /// Получает значение дельты (разницу) между <see cref="OldValue"/> и <see cref="NewValue"/>.
    /// </summary>
    public T Delta => _deltaCalculator(OldValue, NewValue);

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DeltaChangeValue{T}"/>.
    /// </summary>
    /// <param name="oldValue">Старое значение.</param>
    /// <param name="newValue">Новое значение.</param>
    /// <param name="deltaCalculator">Функция для вычисления дельты между старым и новым значением.</param>
    /// <exception cref="ArgumentNullException">Выбрасывается, если <paramref name="deltaCalculator"/> равен <see langword="null"/>.</exception>
    public DeltaChangeValue(T oldValue, T newValue, Func<T, T, T> deltaCalculator)
        : base(oldValue, newValue)
    {
        _deltaCalculator = deltaCalculator ?? throw new ArgumentNullException(nameof(deltaCalculator));
    }

    /// <summary>
    /// Возвращает строковое представление объекта в формате "Δ: {Delta} (Old: {OldValue}, New: {NewValue})".
    /// </summary>
    /// <returns>Строковое представление текущего объекта.</returns>
    public override string ToString() => $"Δ:{Delta} (Old:{OldValue} -> New:{NewValue})";
}

}