using System;
using System.IO;
using System.Text;
using UnityEngine;

#if DEVELOPMENT_BUILD
using UnityEngine.Profiling;
#endif

namespace RedCatEngine.Benchmark
{
	public class BenchmarkRunner : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Интервал обновления FPS (в секундах)")]
		private float _fpsUpdateInterval = 0.5f;
		[SerializeField]
		[Tooltip(
			"Формат имени файла с результатами. "
			+ "{0} - название продукта, "
			+ "{1} - имя сценария,"
			+ "{2} - время запуска бенчмарка")]
		private string _benchmarkNameFormat = "benchmark_results_{0}_{1}_{2}.txt";
		[SerializeField]
		private string _subFolderName = "{0}";
		[SerializeField]
		private float _timeWriteInterval = 10f;
		[SerializeField]
		private Camera _benchmarkCamera;
		private float _fpsAccumulator;
		private int _fpsFramesCount;
		private float _fpsTimeLeft;
		private float _currentFPS;
		private string _benchmarkFilePath;

		private bool _isRunning;
		private float _totalFpsAccumulator;
		private int _totalFpsFramesCount;
		private float _minFps = float.MaxValue;
		private float _maxFps = float.MinValue;
		private float _minDeltaTime = float.MaxValue;
		private float _maxDeltaTime = float.MinValue;

		private float _startTime;
		public bool IsRunning
			=> _isRunning;

		private string GetBenchmarkFolderPath()
		{
			var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			var subFolderName = string.Format(_subFolderName, Application.productName);
			var benchmarkFolderPath = Path.Combine(desktopPath, subFolderName);
			if (!Directory.Exists(benchmarkFolderPath))
				Directory.CreateDirectory(benchmarkFolderPath);

			return benchmarkFolderPath;
		}

		public void StartBenchmark(string scenarioName)
		{
			_benchmarkCamera.gameObject.SetActive(true);
			ResetValues();

			var benchmarkFileName = string.Format(
				_benchmarkNameFormat,
				Application.productName,
				scenarioName,
				DateTime.Now.ToString("yyyy-MM-dd-HH-mm"));

			_benchmarkFilePath = Path.Combine(
				GetBenchmarkFolderPath(),
				benchmarkFileName);
			WriteSystemInfo();
			_fpsTimeLeft = _fpsUpdateInterval;
			_isRunning = true;
			_startTime = Time.time;
		}

		private void ResetValues()
		{
			_minFps = float.MaxValue;
			_maxFps = float.MinValue;
			_minDeltaTime = float.MaxValue;
			_maxDeltaTime = float.MinValue;
			_totalFpsAccumulator = 0;
			_totalFpsFramesCount = 0;
			_fpsTimeLeft = 0;
			_fpsAccumulator = 0f;
			_fpsFramesCount = 0;
			_currentFPS = 0;
		}
		
		private void Update()
		{
			if (!_isRunning)
				return;

			MeasureFPS();
			_minDeltaTime = Mathf.Min(_minDeltaTime, Time.deltaTime);
			_maxDeltaTime = Mathf.Max(_maxDeltaTime, Time.deltaTime);
			if (Time.time % _timeWriteInterval < Time.deltaTime)
				RecordFPS();
		}

		private void WriteSystemInfo()
		{
			var sb = new StringBuilder();
			sb.AppendLine("=== SYSTEM INFORMATION ===");
			sb.AppendLine($"Operating System: {SystemInfo.operatingSystem}");
			sb.AppendLine($"Processor: {SystemInfo.processorType}");
			sb.AppendLine($"Processor Cores: {SystemInfo.processorCount}");
			sb.AppendLine($"Graphics Device: {SystemInfo.graphicsDeviceName}");
			sb.AppendLine($"Graphics Memory (MB): {SystemInfo.graphicsMemorySize}");
			sb.AppendLine($"System Memory (MB): {SystemInfo.systemMemorySize}");
			sb.AppendLine($"Unity Version: {Application.unityVersion}");
			sb.AppendLine($"Screen Resolution: {Screen.currentResolution}");
			sb.AppendLine("\n=== BENCHMARK RESULTS ===");
			sb.AppendLine("Time (s)\tFPS");

			File.WriteAllText(_benchmarkFilePath, sb.ToString());
		}

		private void MeasureFPS()
		{
			_fpsTimeLeft -= Time.deltaTime;
			_fpsAccumulator += Time.timeScale / Time.deltaTime;
			_fpsFramesCount++;

			if (_fpsTimeLeft > 0f)
				return;

			_currentFPS = _fpsAccumulator / _fpsFramesCount;
			_totalFpsAccumulator += _fpsAccumulator;
			_totalFpsFramesCount += _fpsFramesCount;
			_minFps = Mathf.Min(_minFps, _currentFPS);
			_maxFps = Mathf.Max(_maxFps, _currentFPS);
			_fpsTimeLeft = _fpsUpdateInterval;
			_fpsAccumulator = 0f;
			_fpsFramesCount = 0;
		}

		private void RecordFPS()
		{
			var logEntry = $"{(Time.time-_startTime):F1}\t\t{_currentFPS:F1}\n";
			File.AppendAllText(_benchmarkFilePath, logEntry);
		}

		public void StopBenchmark(string reasonName)
		{
			_isRunning = false;
			var sb = new StringBuilder();
			sb.AppendLine("=== BENCHMARK COMPLETED ===");
			sb.AppendLine($"Reason stop: {reasonName}");
			sb.AppendLine($"Min FPS: {_minFps:F1}");
			sb.AppendLine($"Max FPS: {_maxFps:F1}");
			sb.AppendLine($"Arranged FPS: {_totalFpsAccumulator / _totalFpsFramesCount:F1}");
			sb.AppendLine($"Min deltaTime: {_minDeltaTime:F1}");
			sb.AppendLine($"Max deltaTime: {_maxDeltaTime:F1}");

#if UNITY_EDITOR
			Debug.Log("Запущено в редакторе Unity");
			sb.AppendLine(
				$"Total Memory Usage (MB): {SystemInfo.systemMemorySize - (SystemInfo.systemMemorySize - (GC.GetTotalMemory(false) / (1024 * 1024)))}");
			sb.AppendLine(
				$"Allocated Memory (MB): {UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024)}");
			sb.AppendLine(
				$"Reserved Memory (MB): {UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024 * 1024)}");
			sb.AppendLine(
				$"Mono Heap Size (MB): {UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong() / (1024 * 1024)}");
#elif DEVELOPMENT_BUILD
			Debug.Log("Development Build (но не редактор)");
			sb.AppendLine(
				$"Total Memory Usage (MB): {SystemInfo.systemMemorySize - (SystemInfo.systemMemorySize - (GC.GetTotalMemory(false) / (1024 * 1024)))}");
			sb.AppendLine(
				$"Allocated Memory (MB): {UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024)}");
			sb.AppendLine(
				$"Reserved Memory (MB): {UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024 * 1024)}");
			sb.AppendLine(
				$"Mono Heap Size (MB): {UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong() / (1024 * 1024)}");
#else
			Debug.Log("Релизная сборка");
#endif
			Debug.Log("Готово");
			sb.AppendLine("Benchmark completed.");
			File.AppendAllText(_benchmarkFilePath, sb.ToString());
		}
	}
}