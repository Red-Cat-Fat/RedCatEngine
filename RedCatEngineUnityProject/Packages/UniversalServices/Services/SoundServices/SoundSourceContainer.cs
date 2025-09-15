using UnityEngine;

namespace RedCatEngine.CommonServices.Services.SoundServices
{
	public class SoundSourceContainer : MonoBehaviour, ISoundService
	{
		[SerializeField] private AudioSource _musicSource;
		[SerializeField] private AudioSource _sfxSource;

		public void PlayMusic(AudioClip clip)
			=> _musicSource.clip = clip;

		public void PlaySfx(AudioClip clip)
			=> _sfxSource.PlayOneShot(clip);
	}
}