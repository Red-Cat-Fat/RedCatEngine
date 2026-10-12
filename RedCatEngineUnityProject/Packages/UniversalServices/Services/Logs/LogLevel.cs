namespace RedCatEngine.CommonServices.Services.Logs
{
	/// <summary>
	/// Уровень важности сообщения. Порядок значений важен: фильтр сравнивает через >=.
	/// </summary>
	public enum LogLevel : byte
	{
		/// <summary>Трассировка алгоритма. Может срабатывать чаще раза в секунду на объект.</summary>
		Verbose = 0,

		/// <summary>Редкое событие жизненного цикла.</summary>
		Info = 1,

		/// <summary>Восстановимая аномалия, требующая внимания человека.</summary>
		Warning = 2,

		/// <summary>Нарушен контракт, дальнейшее поведение неопределено.</summary>
		Error = 3,

		/// <summary>Только как значение фильтра, не как уровень сообщения.</summary>
		Off = 4
	}
}
