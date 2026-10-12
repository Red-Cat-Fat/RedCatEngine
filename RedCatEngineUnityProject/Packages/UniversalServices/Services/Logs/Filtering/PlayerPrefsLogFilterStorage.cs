using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Logs.Filtering
{
	/// <summary>
	/// Персист фильтра в билде. <paramref name="persistOverrides"/> = false означает,
	/// что пер-теговые оверрайды живут до перезапуска — защита от «включил verbose и забыл».
	/// </summary>
	public sealed class PlayerPrefsLogFilterStorage : ILogFilterStorage
	{
		private const string GlobalKey = "RedCatEngine.Log.Global";
		private const string OverridesKey = "RedCatEngine.Log.Overrides";

		private readonly bool _persistOverrides;

		public PlayerPrefsLogFilterStorage(bool persistOverrides)
			=> _persistOverrides = persistOverrides;

		public bool TryLoad(out string globalLevel, out string overrides)
		{
			globalLevel = null;
			overrides = null;

			if(!PlayerPrefs.HasKey(GlobalKey))
				return false;

			globalLevel = PlayerPrefs.GetString(GlobalKey, string.Empty);
			overrides = _persistOverrides
					? PlayerPrefs.GetString(OverridesKey, string.Empty)
					: string.Empty;

			return true;
		}

		public void Save(string globalLevel, string overrides)
		{
			PlayerPrefs.SetString(GlobalKey, globalLevel ?? string.Empty);
			PlayerPrefs.SetString(OverridesKey, overrides ?? string.Empty);
			PlayerPrefs.Save();
		}

		public void Clear()
		{
			PlayerPrefs.DeleteKey(GlobalKey);
			PlayerPrefs.DeleteKey(OverridesKey);
			PlayerPrefs.Save();
		}
	}
}
