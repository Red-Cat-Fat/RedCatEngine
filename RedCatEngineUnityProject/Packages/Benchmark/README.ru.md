[English](README.md) | **Русский**

# Red Cat Engine: Benchmark

Компонент, который измеряет FPS и память во время игрового сценария и записывает результат в текстовый файл.

| | |
|---|---|
| Идентификатор пакета | `com.boronnikov.games.red-cat-engine.banchmark` |
| Версия | 1.1.0 ([список изменений](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Сборка | `Benchmark` |
| Пространство имён | `RedCatEngine.Benchmark` |

## Установка

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Benchmark
```

## Зависимости

Нет.

## Как это работает

`BenchmarkRunner` — это `MonoBehaviour`. Пока бенчмарк запущен, он каждый кадр замеряет FPS, время кадра и память и с заданным интервалом дописывает строку в отчёт. Отчёт содержит:

- сведения об игре и системе (продукт, версия, ОС, процессор, видеокарта, память, разрешение);
- таблицу: время, FPS, выделенная / зарезервированная память, куча Mono, память GC;
- именованные метки, добавленные во время прогона;
- итог: минимальный / максимальный / средний FPS, минимальное / максимальное время кадра и память.

Файл записывается в `<Рабочий стол>/<название продукта>/benchmark_results_<продукт>_<версия>_<сценарий>_<время>.txt`, поэтому компонент рассчитан на настольные платформы.

Объёмы выделенной и зарезервированной памяти и кучи Mono берутся из Unity Profiler и заполняются только в редакторе или при заданном символе компиляции `CHEAT_ENABLED`; память GC измеряется всегда.

## Использование

1. Добавьте `BenchmarkRunner` на объект сцены.
2. Назначьте **Benchmark Camera** — этот объект включается при старте бенчмарка. Поле обязательное.
3. При необходимости настройте интервал обновления FPS, интервал записи, формат имени файла и имя подпапки.

```csharp
using RedCatEngine.Benchmark;
using UnityEngine;

public class WaveBenchmark : MonoBehaviour
{
	[SerializeField] private BenchmarkRunner _benchmark;

	public void OnWavesStarted()
		=> _benchmark.StartBenchmark("Waves");

	public void OnWaveSpawned(int index)
		=> _benchmark.ActionAddMark($"Wave {index}");

	public void OnWavesFinished()
		=> _benchmark.StopBenchmark("All waves completed");
}
```

`IsRunning` показывает, идёт ли бенчмарк. `ActionAddMark` игнорируется, если бенчмарк не запущен.
