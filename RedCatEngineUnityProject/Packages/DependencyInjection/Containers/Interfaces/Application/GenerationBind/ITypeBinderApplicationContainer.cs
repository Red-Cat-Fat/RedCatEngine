using System;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind
{
	/// <summary>
	/// Интерфейс контейнера, который предоставляет методы для регистрации и создания экземпляров.
	/// </summary>
	public interface ITypeBinderApplicationContainer
	{
		/// <summary>
		/// Привязывает "Dummy" тип к контейнеру, если нормальный тип не найден в текущем контексте.
		/// Используется для обеспечения возможности работы системы при отсутствии нужного реального объекта.
		/// </summary>
		/// <typeparam name="TInstanceBindType">
		/// Тип, к которому будет привязан созданный экземпляр. Это тот тип, который ожидается потребителем.
		/// </typeparam>
		/// <typeparam name="TDummyType">
		/// Тип реализации dummy, который должен наследовать или реализовывать <see cref="TInstanceBindType"/>.
		/// </typeparam>
		/// <param name="context">
		/// Параметры, необходимые для инициализации экземпляра <see cref="TDummyType"/>. 
		/// Передаются в конструктор dummy-класса, если требуется инъекция данных.
		/// </param>
		/// <returns>
		/// Возвращает созданный и зарегистрированный экземпляр типа <see cref="TInstanceBindType"/>.
		/// </returns>
		TInstanceBindType BindDummy<TInstanceBindType, TDummyType>(params object[] context)
			where TDummyType : TInstanceBindType;

		/// <summary>
		/// Регистрирует и возвращает уникальный экземпляр <typeparamref name="TInstanceType"/>, привязанный к типу <typeparamref name="TBindType"/>.
		/// </summary>
		/// <param name="context">
		/// Массив объектов, передаваемый в конструктор <typeparamref name="TInstanceType"/> для его инициализации.
		/// Может использоваться для внедрения зависимостей или параметров при создании экземпляра.
		/// </param>
		/// <typeparam name="TBindType">
		/// Тип, к которому будет произведена привязка. Это тип, который будет запрашиваться из контейнера.
		/// </typeparam>
		/// <typeparam name="TInstanceType">
		/// Тип реализации, который создаётся и связывается с <typeparamref name="TBindType"/>.
		/// Должен быть наследником или реализацией <typeparamref name="TBindType"/>.
		/// </typeparam>
		/// <returns>
		/// Созданный и зарегистрированный экземпляр типа <typeparamref name="TInstanceType"/>,
		/// доступный по интерфейсу <typeparamref name="TBindType"/>.
		/// </returns>
		TBindType BindType<TBindType, TInstanceType>(params object[] context)
			where TInstanceType : TBindType;

		/// <summary>
		/// Регистрирует и возвращает экземпляр <typeparamref name="TInstanceType"/>
		/// как один из элементов массива экземпляров <typeparamref name="TBindArrayType"/>
		/// </summary>
		/// <param name="context">
		/// Массив объектов, передаваемый в конструктор <typeparamref name="TInstanceType"/> для его инициализации.
		/// Может использоваться для внедрения зависимостей или параметров при создании экземпляра.
		/// </param>
		/// <typeparam name="TBindArrayType">
		/// Тип, к массиву экземпляров которому будет произведена привязка. Это тип массива, который будет запрашиваться из контейнера.
		/// </typeparam>
		/// <typeparam name="TInstanceType">
		/// Тип реализации, который создаётся и связывается с <typeparamref name="TBindArrayType"/>.
		/// Должен быть наследником или реализацией <typeparamref name="TBindArrayType"/>.
		/// </typeparam>
		/// <returns>
		/// Созданный и зарегистрированный экземпляр типа <typeparamref name="TBindArrayType"/>.
		/// </returns>
		TBindArrayType BindArrayType<TBindArrayType, TInstanceType>(params object[] context)
			where TInstanceType : TBindArrayType;

		/// <summary>
		/// Регистрирует и возвращает новый уникальный экземпляр типа <typeparamref name="TInstanceBindType"/>.
		/// </summary>
		/// <param name="context">
		/// Массив объектов, передаваемый в конструктор <typeparamref name="TInstanceBindType"/> для его инициализации.
		/// Может использоваться для внедрения зависимостей или параметров при создании экземпляра.
		/// </param>
		/// <typeparam name="TInstanceBindType">
		/// Тип, экземпляр которого создаётся и регистрируется в контейнере.
		/// </typeparam>
		/// <returns>
		/// Созданный экземпляр типа <typeparamref name="TInstanceBindType"/>.
		/// </returns>
		TInstanceBindType BindType<TInstanceBindType>(params object[] context);

		/// <summary>
		/// Регистрирует и возвращает новый уникальный экземпляр указанного типа.
		/// </summary>
		/// <param name="type">
		/// Тип, экземпляр которого нужно создать и зарегистрировать.
		/// </param>
		/// <param name="context">
		/// Массив объектов, передаваемый в конструктор указанного типа для его инициализации.
		/// Может использоваться для внедрения зависимостей или параметров при создании экземпляра.
		/// </param>
		/// <returns>
		/// Созданный экземпляр указанного типа в виде объекта <see cref="object"/>.
		/// </returns>
		object BindType(Type type, params object[] context);

		/// <summary>
		/// Регистрирует как элемент массива экземпляров и привязывает его к типу <typeparamref name="TInstanceBindType"/>.
		/// </summary>
		/// <param name="context">
		/// Массив объектов, передаваемый в конструктор экземпляров для их инициализации.
		/// Может использоваться для внедрения зависимостей или параметров при создании экземпляра.
		/// </param>
		/// <typeparam name="TInstanceBindType">
		/// Тип массива, к которому будет привязан созданный экземпляр.
		/// </typeparam>
		/// <returns>
		/// Созданный и зарегистрированный экземпляр типа <typeparamref name="TInstanceBindType"/>.
		/// </returns>
		TInstanceBindType BindArrayType<TInstanceBindType>(params object[] context);

		/// <summary>
		/// Регистрирует как элемент массива экземпляров и привязывает его к типу, указанному в качестве переменной <paramref name="type"/>.
		/// </summary>
		/// <param name="type">
		/// Тип, экземпляр которого будет создан и зарегистрирован в качестве элемента массива.
		/// </param>
		/// <param name="context">
		/// Массив объектов, передаваемый в конструктор экземпляров для их инициализации.
		/// Может использоваться для внедрения зависимостей или параметров при создании экземпляра.
		/// </param>
		/// <returns>
		/// Созданный и зарегистрированный экземпляр в виде объекта <see cref="object"/>.
		/// </returns>
		object BindArrayType(Type type, params object[] context);
	}
}