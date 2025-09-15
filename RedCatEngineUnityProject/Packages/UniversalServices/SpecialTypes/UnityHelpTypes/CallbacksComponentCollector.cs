using System;
using RedCatEngine.CommonServices.SpecialTypes.UnityHelpTypes.ComponentFinders;

namespace RedCatEngine.CommonServices.SpecialTypes.UnityHelpTypes
{
	/// <summary>
/// Расширяет <see cref="ComponentCollector{TBehaviour}"/>, добавляя возможность вызывать пользовательские коллбэки
/// после добавления или удаления компонента из списка.
/// </summary>
/// <typeparam name="TBehaviour">Тип компонентов, которые собираются. Может быть интерфейсом, классом или другим типом.</typeparam>
public class CallbacksComponentCollector<TBehaviour> : ComponentCollector<TBehaviour> where TBehaviour : class
{
    /// <summary>
    /// Действие, которое будет вызвано после успешного добавления компонента в список.
    /// </summary>
    private readonly Action<TBehaviour> _callbackAfterAddComponent;

    /// <summary>
    /// Действие, которое будет вызвано после успешного удаления компонента из списка.
    /// </summary>
    private readonly Action<TBehaviour> _callbackAfterRemoveComponent;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="CallbacksComponentCollector{TBehaviour}"/>.
    /// </summary>
    /// <param name="callbackAfterAddComponent">
    /// Коллбэк, вызываемый после добавления компонента.
    /// </param>
    /// <param name="callbackAfterRemoveComponent">
    /// Коллбэк, вызываемый после удаления компонента.
    /// </param>
    public CallbacksComponentCollector(
        Action<TBehaviour> callbackAfterAddComponent,
        Action<TBehaviour> callbackAfterRemoveComponent)
    {
        _callbackAfterAddComponent = callbackAfterAddComponent;
        _callbackAfterRemoveComponent = callbackAfterRemoveComponent;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="CallbacksComponentCollector{TBehaviour}"/>.
    /// </summary>
    /// <param name="specialFinder">Переопределённый поисковик, в который можно добавить разные фильтры</param>
    /// <param name="callbackAfterAddComponent">
    /// Коллбэк, вызываемый после добавления компонента.
    /// </param>
    /// <param name="callbackAfterRemoveComponent">
    /// Коллбэк, вызываемый после удаления компонента.
    /// </param>
    public CallbacksComponentCollector(
        ComponentTypeFinder<TBehaviour> specialFinder,
        Action<TBehaviour> callbackAfterAddComponent,
        Action<TBehaviour> callbackAfterRemoveComponent
    )
        : base(specialFinder)
    {
        _callbackAfterAddComponent = callbackAfterAddComponent;
        _callbackAfterRemoveComponent = callbackAfterRemoveComponent;
    }
    /// <summary>
    /// Вызывается после добавления компонента в список.
    /// Выполняет действие <see cref="_callbackAfterAddComponent"/>.
    /// </summary>
    /// <param name="component">Добавленный компонент.</param>
    protected override void DoAfterAdd(TBehaviour component)
        => _callbackAfterAddComponent?.Invoke(component);

    /// <summary>
    /// Вызывается после удаления компонента из списка.
    /// Выполняет действие <see cref="_callbackAfterRemoveComponent"/>.
    /// </summary>
    /// <param name="component">Удалённый компонент.</param>
    protected override void DoAfterRemove(TBehaviour component)
        => _callbackAfterRemoveComponent?.Invoke(component);
}

}