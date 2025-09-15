using RedCatEngine.Configs;
using RedCatEngine.GameSettings.TypeSettings;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.GameSettings
{
	[CreateAssetMenu(fileName = nameof(GameSettingsConfig), menuName = "Configs/Common/Game Settings Config")]
	public class GameSettingsConfig : BaseSingleConfig<GameSettingsConfig>
	{
		[SR][SerializeReference]
		public BaseGameSetting[] BaseSettings;
	}
}