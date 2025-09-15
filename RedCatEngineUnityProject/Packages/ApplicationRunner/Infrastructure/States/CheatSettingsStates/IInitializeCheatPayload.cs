using UnityEngine;

namespace RedCatEngine.ApplicationRunner.Infrastructure.States.CheatSettingsStates
{
	public interface IInitializeCheatPayload
	{
		GameObject ConsolePrefab { get; }
	}
}