namespace RedCatEngine.CommonServices.SpecialTypes.ChangedValues
{
    /// <summary>
    /// Представляет информацию о изменении значения, содержащую старое и новое значение.
    /// </summary>
    /// <typeparam name="T">Тип хранимого значения.</typeparam>
    public class ChangeValue<T>
    {
        /// <summary>
        /// Получает старое значение до изменения.
        /// </summary>
        public T OldValue { get; }

        /// <summary>
        /// Получает новое значение после изменения.
        /// </summary>
        public T NewValue { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ChangeValue{T}"/>.
        /// </summary>
        /// <param name="oldValue">Старое значение.</param>
        /// <param name="newValue">Новое значение.</param>
        public ChangeValue(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Возвращает строковое представление объекта в формате "(Old: {OldValue}, New: {NewValue})".
        /// </summary>
        /// <returns>Строковое представление текущего объекта.</returns>
        public override string ToString() => $"(Old:{OldValue} -> New:{NewValue})";
    }
}
