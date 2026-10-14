using System.Collections.Generic;
using NUnit.Framework;
using RedCatEngine.CommonServices.Services.Logs;
using RedCatEngine.CommonServices.Services.Save.Applier;
using RedCatEngine.CommonServices.Services.Save.Realisations;
using RedCatEngine.CommonServices.Services.Save.SaveReader;

namespace RedCatEngine.UniversalServices.Tests
{
	public class SaveLoadReaderServiceTests
	{
		private class MemorySaveLoadReaderService : BaseSaveLoadReaderService
		{
			public readonly Dictionary<string, string> Storage = new();

			public MemorySaveLoadReaderService(ISaveApplierContainer applier)
				: base(applier, NullLogService.Instance) { }

			protected override string GetJsonForLoad(string key)
				=> Storage.TryGetValue(key, out var json) ? json : null;

			protected override bool SetJsonForSave(string key, string loadJson)
			{
				Storage[key] = loadJson;
				return true;
			}
		}

		private class CounterService : ISaved
		{
			public int Value;
			public readonly List<string> LoadedJson = new();

			public string SaveKey => "Counter";

			public string GetSerializedSaveData()
				=> Value.ToString();

			public void Load(string serializedSaveData)
			{
				LoadedJson.Add(serializedSaveData);
				Value = string.IsNullOrEmpty(serializedSaveData) ? 0 : int.Parse(serializedSaveData);
			}
		}

		private CounterService _counter;
		private MemorySaveLoadReaderService _reader;

		[SetUp]
		public void SetUp()
		{
			_counter = new CounterService();
			var applier = new SaveApplierContainer();
			applier.Attach(_counter);
			_reader = new MemorySaveLoadReaderService(applier);
		}

		[Test]
		public void GivenNoSave_WhenLoad_ThenServicesGetNullAndSaveIsCreated()
		{
			Assert.DoesNotThrow(() => _reader.Load("Slot"));

			Assert.AreEqual(1, _counter.LoadedJson.Count, "Service must be loaded exactly once");
			Assert.IsNull(_counter.LoadedJson[0], "First launch must load services with null");
			Assert.IsTrue(_reader.Storage.ContainsKey("Slot"), "Initial save must be written");
		}

		[Test]
		public void GivenSavedState_WhenLoad_ThenServiceRestored()
		{
			_counter.Value = 7;
			_reader.SaveCurrentState("Slot");
			_counter.Value = 0;

			_reader.Load("Slot");

			Assert.AreEqual(7, _counter.Value);
		}
	}
}
