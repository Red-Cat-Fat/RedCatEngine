**English** | [Русский](README.ru.md)

# Red Cat Engine: Benchmark

A component that measures FPS and memory during a gameplay scenario and writes the result to a text file.

| | |
|---|---|
| Package id | `com.boronnikov.games.red-cat-engine.banchmark` |
| Version | 1.1.0 ([changelog](CHANGELOG.md)) |
| Unity | 2021.3+ |
| Assembly | `Benchmark` |
| Namespace | `RedCatEngine.Benchmark` |

## Installation

Window > Package Manager > **+** > *Add package from git URL*:

```text
https://github.com/Red-Cat-Fat/RedCatEngine.git?path=/RedCatEngineUnityProject/Packages/Benchmark
```

## Dependencies

None.

## How it works

`BenchmarkRunner` is a `MonoBehaviour`. While a benchmark is running it samples FPS, frame time and memory every frame and appends a line to the report at a fixed interval. The report contains:

- game and system information (product, version, OS, CPU, GPU, memory, resolution);
- a table of time, FPS, allocated / reserved / Mono heap / GC memory;
- named marks you add during the run;
- a summary with min / max / average FPS, min / max frame time and memory.

The file is written to `<Desktop>/<product name>/benchmark_results_<product>_<version>_<scenario>_<time>.txt`, so the component is intended for desktop platforms.

Allocated, reserved and Mono heap sizes are read from the Unity Profiler and are filled only in the Editor or when the `CHEAT_ENABLED` scripting define is set; GC memory is always measured.

## Usage

1. Add `BenchmarkRunner` to a scene object.
2. Assign **Benchmark Camera** — the object is activated when a benchmark starts. The field is required.
3. Optionally adjust the FPS update interval, the write interval, the file name format and the sub-folder name.

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

`IsRunning` tells whether a benchmark is in progress. `ActionAddMark` is ignored when nothing is running.
