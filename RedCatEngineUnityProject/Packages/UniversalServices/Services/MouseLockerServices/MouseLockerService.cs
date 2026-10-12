using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.MouseLockerServices
{
	public class MouseLockerService : IMouseLockerService
	{
		private readonly List<object> _showTriggers = new();

		public void Hide(object trigger)
		{
			if (_showTriggers.Contains(trigger))
				_showTriggers.Remove(trigger);
			UpdateState();
		}

		public void Show(object trigger)
		{
			_showTriggers.Add(trigger);
			UpdateState();
		}

		private void UpdateState()
		{
			if (_showTriggers.Any())
			{
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}
	}
}