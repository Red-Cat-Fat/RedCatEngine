using UnityEngine;

namespace RedCatEngine.CommonServices.Services.SoundServices
{
	public class SoundService : ISoundService
	{
		private readonly AudioSource _musicSource;
		private readonly AudioSource _sfxSource;

		public SoundService(AudioSource musicSource,AudioSource sfxSource)
		{
			_musicSource = musicSource;
			_sfxSource = sfxSource;
		}
		
		public void PlayMusic(AudioClip clip)
			=> _musicSource.clip = clip;

		public void PlaySfx(AudioClip clip)
			=> _sfxSource.PlayOneShot(clip);
	}
}