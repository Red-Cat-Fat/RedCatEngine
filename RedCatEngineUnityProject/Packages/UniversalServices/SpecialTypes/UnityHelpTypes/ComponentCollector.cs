using System.Collections;
using System.Collections.Generic;
using RedCatEngine.CommonServices.SpecialTypes.UnityHelpTypes.ComponentFinders;
using UnityEngine;

namespace RedCatEngine.CommonServices.SpecialTypes.UnityHelpTypes
{
	/// <summary>
	/// Утилита для сбора и управления компонентами указанного типа на игровых объектах.
	/// </summary>
	/// <typeparam name="TBehaviour">Тип компонента, который собирается.</typeparam>
	public class ComponentCollector<TBehaviour> : IEnumerable where TBehaviour : class
	{
		/// <summary>
		/// Вспомогательный фасад для проверки наличия компонентов нужного типа.
		/// </summary>
		private readonly ComponentTypeFinder<TBehaviour> _specialFinder;

		/// <summary>
		/// Список собранных компонентов типа <typeparamref name="TBehaviour"/>.
		/// </summary>
		private readonly List<TBehaviour> _components = new();

		public ComponentCollector()
		{
			_specialFinder = new ComponentTypeFinder<TBehaviour>();
		}

		public ComponentCollector(ComponentTypeFinder<TBehaviour> specialFinder)
		{
			_specialFinder = specialFinder;
		}

		/// <summary>
		/// Пытается добавить компонент указанного типа из игрового объекта в список.
		/// </summary>
		/// <param name="gameObject">Игровой объект, на котором проверяется наличие компонента.</param>
		/// <param name="component">Найденный и успешно добавленный компонент. Пустой в случае отсутствия компонента на объекте</param>
		/// <returns>
		/// <see langword="true"/>, если компонент был найден и успешно добавлен;
		/// <see langword="false"/>, если компонент не найден или уже существует в списке.
		/// </returns>
		public bool TryAdd(GameObject gameObject, out TBehaviour component)
		{
			if (!_specialFinder.IsHasComponent(gameObject, out component))
				return false;

			_components.Add(component);
			DoAfterAdd(component);
			return true;
		}

		/// <summary>
		/// Дополнительные действия после добавления компонента. Можно переопределить в наследниках при необходимости.
		/// </summary>
		/// <param name="component">Компонент над которым необходимо совершить дополнительные действия после добавления.</param>
		protected virtual void DoAfterAdd(TBehaviour component)
		{
		}

		/// <summary>
		/// Пытается удалить компонент указанного типа из списка, если он там есть.
		/// </summary>
		/// <param name="gameObject">Игровой объект, чей компонент нужно удалить.</param>
		/// <param name="component">Найденный и успешно удалённый компонент. Пустой в случае отсутствия компонента на объекте</param>
		/// <returns>
		/// <see langword="true"/>, если компонент был найден и успешно удален;
		/// <see langword="false"/>, если компонент не найден или отсутствует в коллекции.
		/// </returns>
		public bool TryRemove(GameObject gameObject, out TBehaviour component)
		{
			return _specialFinder.IsHasComponent(gameObject, out component) && Remove(component);
		}

		/// <summary>
		/// Пытается удалить указанный компонент из коллекции, если он в ней содержится.
		/// </summary>
		/// <param name="component">Компонент, который нужно удалить из коллекции.</param>
		/// <returns>
		/// <see langword="true"/>, если компонент был в коллекции и успешно удалён;
		/// <see langword="false"/>, если компонент не был найден в коллекции.
		/// </returns>
		public bool TryRemove(TBehaviour component)
			=> Remove(component);

		private bool Remove(TBehaviour component)
		{
			if (!_components.Contains(component))
				return false;

			_components.Remove(component);
			DoAfterRemove(component);
			return true;
		}

		/// <summary>
		/// Дополнительные действия после удаления компонента. Можно переопределить в наследниках при необходимости.
		/// </summary>
		/// <param name="component">Компонент над которым необходимо совершить дополнительные действия после удаления.</param>
		protected virtual void DoAfterRemove(TBehaviour component)
		{
		}

		/// <summary>
		/// Возвращает итератор по компонентам.
		/// </summary>
		/// <returns>
		/// Итератор по компонентам.
		/// </returns>
		public IEnumerator GetEnumerator()
			=> _components.GetEnumerator();

		/// <summary>
		/// Очищает список компонентов.
		/// </summary>
		public void Clear()
		{
			var componentsArray = _components.ToArray();
			foreach (var component in componentsArray)
			{
				Remove(component);
			}
		}
	}
}