using UnityEngine;

namespace RedCatEngine.CommonServices.Utils
{
	public class FPSCounter : MonoBehaviour
	{
		private float _deltaTime;

		private void Update() 
			=> _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;

		private void OnGUI()
		{
			int w = Screen.width, h = Screen.height;
			var style = new GUIStyle();
			var rect = new Rect(0,
				0,
				w,
				h * 2f / 100);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = h * 2 / 100;
			style.normal.textColor = Color.white;
			var msec = _deltaTime * 1000.0f;
			var fps = 1.0f / _deltaTime;
			var text = $"{msec:0.0} ms ({fps:0.} fps)";
			GUI.Label(rect,
				text,
				style);
		}
	}
}