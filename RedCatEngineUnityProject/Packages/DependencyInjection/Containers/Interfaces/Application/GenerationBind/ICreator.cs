using System;
using RedCatEngine.DependencyInjection.Specials;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind
{
	/// <summary>
	/// Интерфейс механизма создания экземпляров классов
	/// </summary>
	public interface ICreator
	{
		/// <summary>
		/// Экземпляр инжектора, который используется для создания объектов
		/// </summary>
		Injector Injector { get; }

		/// <summary>
		/// Создаёт экземпляр класса, без биндинга к контейнеру
		/// </summary>
		/// <param name="type">Тип объекта, который необходимо создать</param>
		/// <param name="context">Контекст, необходимый для инициализации объекта указанного класса</param>
		/// <returns></returns>
		object Create(Type type, params object[] context);

		/// <summary>
		/// Создаёт экземпляр класса, без биндинга к контейнеру
		/// </summary>
		/// <param name="context">Контекст, необходимый для инициализации объекта указанного класса</param>
		/// <typeparam name="T">Тип объекта, который необходимо создать</typeparam>
		/// <returns></returns>
		T Create<T>(params object[] context);
	}
}