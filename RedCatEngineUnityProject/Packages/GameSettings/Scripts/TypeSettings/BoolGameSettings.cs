namespace RedCatEngine.GameSettings.TypeSettings
{
	public abstract class BoolGameSettings : BaseGameSetting
	{
		public bool Value;

		protected BoolGameSettings(bool value)
		{
			Value = value;
		}
	}
}