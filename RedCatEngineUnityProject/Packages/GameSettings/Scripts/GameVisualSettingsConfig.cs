using RedCatEngine.Configs;
using RedCatEngine.GameSettings.TypeSettings;
using SerializeReferenceEditor;
using UnityEngine;

namespace RedCatEngine.GameSettings
{
	[CreateAssetMenu(fileName = nameof(GameVisualSettingsConfig), menuName = "Configs/Common/Game Settings Config")]
	public class GameVisualSettingsConfig : BaseSingleConfig<GameVisualSettingsConfig>
	{
		[SR][SerializeReference]
		public BaseGameSetting[] BaseSettings;
	}
}