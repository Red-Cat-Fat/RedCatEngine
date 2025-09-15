using UnityEngine;

namespace RedCatEngine.CommonServices.SpecialTypes.UnityHelpTypes.ComponentFinders
{
	/// <summary>
	/// Утилита для проверки наличия компонента определённого типа на объекте.
	/// </summary>
	/// <typeparam name="TBehaviour">Тип компонента, который необходимо найти. Должен быть наследником <see cref="MonoBehaviour"/>.</typeparam>
	public class ComponentTypeFinder<TBehaviour> where TBehaviour : class
	{
		/// <summary>
		/// Проверяет, содержит ли указанный объект <see cref="MonoBehaviour"/> компонент типа <typeparamref name="TBehaviour"/>.
		/// </summary>
		/// <param name="otherComponent">Компонент, чей объект будет проверяться на наличие нужного компонента.</param>
		/// <param name="component">Найденный компонент типа <typeparamref name="TBehaviour"/>, если он существует.</param>
		/// <returns>
		/// <see langword="true"/>, если компонент найден; в противном случае — <see langword="false"/>.
		/// </returns>
		public bool IsHasComponent(MonoBehaviour otherComponent, out TBehaviour component)
		{
			component = null;
			return otherComponent != null
					&& otherComponent.gameObject.TryGetComponent(out component)
					&& IsValid(otherComponent.gameObject, component);
		}

		/// <summary>
		/// Проверяет, содержит ли указанный объект <see cref="GameObject"/> компонент типа <typeparamref name="TBehaviour"/>.
		/// </summary>
		/// <param name="gameObject">Объект, на котором ищется компонент.</param>
		/// <param name="component">Найденный компонент типа <typeparamref name="TBehaviour"/>, если он существует.</param>
		/// <returns>
		/// <see langword="true"/>, если компонент найден; в противном случае — <see langword="false"/>.
		/// </returns>
		public bool IsHasComponent(GameObject gameObject, out TBehaviour component)
		{
			component = null;
			return gameObject != null && gameObject.TryGetComponent(out component) && IsValid(gameObject, component);
		}

		/// <summary>
		/// Валидирует, должен ли данный объект игнорироваться при поиске компонента.
		/// </summary>
		/// <param name="gameObject">Объект, на котором осуществлялся поиск компонента</param>
		/// <param name="component"></param>
		/// <returns>Прошла ли валидация успешно</returns>
		protected virtual bool IsValid(GameObject gameObject, TBehaviour component) => true;
	}
}