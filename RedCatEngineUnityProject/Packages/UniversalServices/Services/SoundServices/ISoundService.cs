using UnityEngine;

namespace RedCatEngine.CommonServices.Services.SoundServices
{
	public interface ISoundService
	{
		void PlayMusic(AudioClip clip);
		void PlaySfx(AudioClip clip);
	}
}