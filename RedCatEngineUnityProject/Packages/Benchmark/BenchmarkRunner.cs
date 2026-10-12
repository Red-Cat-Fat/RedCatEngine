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
			+ "{3} - версия проекта"
			+ "{1} - имя сценария,"
			+ "{2} - время запуска бенчмарка")]
		private string _benchmarkNameFormat = "benchmark_results_{0}_{3}_{1}_{2}.txt";
		[SerializeField]
		private string _subFolderName = "{0}";
		[SerializeField]
		private float _timeWriteInterval = 10f;
		[SerializeField]
		private GameObject _benchmarkCamera;
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

		private float _currentAllocatedMB;
		private float _currentReservedMB;
		private float _currentMonoHeapMB;
		private float _currentGCMB;

		private float _minAllocatedMB = float.MaxValue;
		private float _maxAllocatedMB = float.MinValue;
		private float _minReservedMB = float.MaxValue;
		private float _maxReservedMB = float.MinValue;
		private float _minMonoHeapMB = float.MaxValue;
		private float _maxMonoHeapMB = float.MinValue;
		private float _minGCMB = float.MaxValue;
		private float _maxGCMB = float.MinValue;

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
				DateTime.Now.ToString("yyyy-MM-dd_HH-mm"),
				Application.version);

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

			_minAllocatedMB = float.MaxValue;
			_maxAllocatedMB = float.MinValue;
			_minReservedMB = float.MaxValue;
			_maxReservedMB = float.MinValue;
			_minMonoHeapMB = float.MaxValue;
			_maxMonoHeapMB = float.MinValue;
			_minGCMB = float.MaxValue;
			_maxGCMB = float.MinValue;
		}
		
		private void Update()
		{
			if (!_isRunning)
				return;

			MeasureFPS();
			MeasureMemory();
			_minDeltaTime = Mathf.Min(_minDeltaTime, Time.deltaTime);
			_maxDeltaTime = Mathf.Max(_maxDeltaTime, Time.deltaTime);
			if (Time.time % _timeWriteInterval < Time.deltaTime)
				RecordFPS();
		}

		private void WriteSystemInfo()
		{
			var sb = new StringBuilder();
			sb.AppendLine("=== GAME INFORMATION ===");
			sb.AppendLine($"Project name: {Application.productName}");
			sb.AppendLine($"Version: {Application.version}");
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
			sb.AppendLine("Time (s)\tFPS\t\tAllocMB\tReservedMB\tMonoHeapMB\tGCMB");

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

		private void MeasureMemory()
		{
#if UNITY_EDITOR || CHEAT_ENABLED
			_currentAllocatedMB = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
			_currentReservedMB = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);
			_currentMonoHeapMB = UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong() / (1024f * 1024f);
			_minAllocatedMB = Mathf.Min(_minAllocatedMB, _currentAllocatedMB);
			_maxAllocatedMB = Mathf.Max(_maxAllocatedMB, _currentAllocatedMB);
			_minReservedMB = Mathf.Min(_minReservedMB, _currentReservedMB);
			_maxReservedMB = Mathf.Max(_maxReservedMB, _currentReservedMB);
			_minMonoHeapMB = Mathf.Min(_minMonoHeapMB, _currentMonoHeapMB);
			_maxMonoHeapMB = Mathf.Max(_maxMonoHeapMB, _currentMonoHeapMB);
#else
			_currentAllocatedMB = 0;
			_currentReservedMB = 0;
			_currentMonoHeapMB = 0;
#endif
			_currentGCMB = GC.GetTotalMemory(false) / (1024f * 1024f);
			_minGCMB = Mathf.Min(_minGCMB, _currentGCMB);
			_maxGCMB = Mathf.Max(_maxGCMB, _currentGCMB);
		}

		private void RecordFPS()
		{
			var logEntry = $"{(Time.time - _startTime):F1}\t\t{_currentFPS:F1}\t\t{_currentAllocatedMB:F0}\t{_currentReservedMB:F0}\t\t{_currentMonoHeapMB:F0}\t\t{_currentGCMB:F1}\n";
			File.AppendAllText(_benchmarkFilePath, logEntry);
		}

		private void AppendMemorySummary(StringBuilder sb)
		{
			bool hasProfiler = false;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			hasProfiler = true;
#endif

			sb.AppendLine("\n=== MEMORY SUMMARY ===");

			if (hasProfiler)
			{
				sb.AppendLine($"Min Allocated Memory (MB): {_minAllocatedMB:F0}");
				sb.AppendLine($"Max Allocated Memory (MB): {_maxAllocatedMB:F0}");
				sb.AppendLine($"Min Reserved Memory (MB): {_minReservedMB:F0}");
				sb.AppendLine($"Max Reserved Memory (MB): {_maxReservedMB:F0}");
				sb.AppendLine($"Min Mono Heap Size (MB): {_minMonoHeapMB:F0}");
				sb.AppendLine($"Max Mono Heap Size (MB): {_maxMonoHeapMB:F0}");
			}

			sb.AppendLine($"Min GC Memory (MB): {_minGCMB:F1}");
			sb.AppendLine($"Max GC Memory (MB): {_maxGCMB:F1}");
			sb.AppendLine($"Total System Memory (MB): {SystemInfo.systemMemorySize}");
		}

		public void ActionAddMark(string markName)
		{
			if (!_isRunning)
				return;

			var logEntry = $"{(Time.time - _startTime):F1} Mark:\t\t{markName}\n";
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

			AppendMemorySummary(sb);

#if UNITY_EDITOR
			Debug.Log("Запущено в редакторе Unity");
#elif DEVELOPMENT_BUILD
			Debug.Log("Development Build (но не редактор)");
#else
			Debug.Log("Релизная сборка");
#endif
			Debug.Log("Готово");
			sb.AppendLine("Benchmark completed.");
			File.AppendAllText(_benchmarkFilePath, sb.ToString());
		}
	}
}