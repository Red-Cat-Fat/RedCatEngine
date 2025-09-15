using JetBrains.Annotations;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Specials.Components;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.SoundServices
{
	public class SoundSfxContainer : MonoConstruct, ISoundContainer
	{
		[SerializeField]
		private AudioClip _sfx;

		private ISoundService _soundService;

		[MonoInject]
		public void Construct(ISoundService soundService)
		{
			_soundService = soundService;
		}

		[UsedImplicitly]
		public void ActionPlay()
			=> _soundService.PlaySfx(_sfx);

		public void SetClip(AudioClip audioClip)
		{
			_sfx = audioClip;
		}
	}
}