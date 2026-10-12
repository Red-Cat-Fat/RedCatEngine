using RedCatEngine.CommonServices.Services.Save.Data;

namespace RedCatEngine.CommonServices.Services.Save.SaveReader
{
	/// <summary>
	/// Системный сервис для сохранения и загрузки данных, по факту собирает в себе все сервисы с ISaved
	/// и по команде загружает их или записывает их данные в JSON
	/// </summary>
	public interface ISaveLoadReaderService
	{
		void Load(string key);

		/// <summary>Читает и парсит сохранение, не применяя его ни к одному ISaveApplierContainer.
		/// Нужен вызывающим, которые сами решают, кому раздать результат (несколько scope'ов).
		/// Возвращает null, если по ключу пусто.</summary>
		SaveData LoadData(string key);

		/// <summary>Возвращает true тогда и только тогда, когда файл физически записан (И7).</summary>
		bool Save(string key, SaveData saveData);
		/// <summary>Возвращает true тогда и только тогда, когда файл физически записан (И7).</summary>
		bool SaveCurrentState(string key);
	}
}
