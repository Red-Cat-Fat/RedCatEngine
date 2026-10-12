namespace RedCatEngine.DependencyInjection.Specials.Diagnostics
{
	/// <summary>
	/// Трассировка жизненного цикла контейнера (конструирование компонентов, дочерние контейнеры)
	/// и ошибки загрузки типов в кэше.
	/// Пакет не может ссылаться на систему логирования проекта (цикл asmdef), поэтому
	/// реализация-мост передаётся в конструктор корневого контейнера и наследуется дочерними;
	/// без явной передачи контейнер молчит (<see cref="NullContainerDiagnostics"/>).
	/// </summary>
	public interface IContainerDiagnostics
	{
		void Trace(string format, params object[] args);
		void Error(string format, params object[] args);
	}
}
