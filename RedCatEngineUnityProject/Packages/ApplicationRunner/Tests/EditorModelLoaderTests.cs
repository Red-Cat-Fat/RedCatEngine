using System;
using NUnit.Framework;
using RedCatEngine.ApplicationRunner.Meta.PlayerModels.ModelLoaders;
using UnityEngine;

namespace RedCatEngine.ApplicationRunner.Tests
{
	public class EditorModelLoaderTests
	{
		private const string SaveKey = "GameSave";

		private bool _hadSave;
		private string _savedBackup;

		[Serializable]
		public class TestModel
		{
			public int Coins;
		}

		[SetUp]
		public void SetUp()
		{
			_hadSave = PlayerPrefs.HasKey(SaveKey);
			_savedBackup = PlayerPrefs.GetString(SaveKey);
			PlayerPrefs.DeleteKey(SaveKey);
		}

		[TearDown]
		public void TearDown()
		{
			if (_hadSave)
				PlayerPrefs.SetString(SaveKey, _savedBackup);
			else
				PlayerPrefs.DeleteKey(SaveKey);
		}

		[Test]
		public void GivenSavedModel_WhenLoad_ThenCallbackInvokedOnceWithSavedModel()
		{
			var loader = new EditorModelLoader<TestModel>();
			loader.Save(new TestModel { Coins = 42 });

			var calls = 0;
			TestModel loaded = null;
			loader.Load(model =>
			{
				calls++;
				loaded = model;
			});

			Assert.AreEqual(1, calls, "Load callback must be invoked exactly once");
			Assert.AreEqual(42, loaded.Coins, "Saved model was replaced");
		}

		[Test]
		public void GivenNoSave_WhenLoad_ThenCallbackInvokedOnceWithNewModel()
		{
			var loader = new EditorModelLoader<TestModel>();

			var calls = 0;
			TestModel loaded = null;
			loader.Load(model =>
			{
				calls++;
				loaded = model;
			});

			Assert.AreEqual(1, calls, "Load callback must be invoked exactly once");
			Assert.IsNotNull(loaded);
			Assert.AreEqual(0, loaded.Coins);
		}
	}
}
