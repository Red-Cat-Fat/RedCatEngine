using System;
using UnityEngine;

namespace RedCatEngine.CommonServices.Services.Times.GameDayTimeServices.Monobeshs
{
	public class SunRotator : MonoBehaviour
	{
		[SerializeField] private Vector3 _axisRotation = Vector3.right;
		[SerializeField] private Light _sun;
		[SerializeField] private Light _moon;
		[SerializeField] private AnimationCurve _lightIntensityCurve;
		[SerializeField] private float _maxSunIntensity = 1;
		[SerializeField] private float _maxMoonIntensity = 0.5f;
		[SerializeField] private Color _dayAmbientLight;
		[SerializeField] private Color _nightAmbientLight;
		//[SerializeField] private Volume _volume;
		[SerializeField] private Material _skyboxMaterial;

		//private ColorAdjustments _colorAdjustments;

		public event Action OnSunrise
		{
			add => _service.SunriseEvent += value;
			remove => _service.SunriseEvent -= value;
		}

		public event Action OnSunset
		{
			add => _service.SunsetEvent += value;
			remove => _service.SunsetEvent -= value;
		}

		public event Action OnHourChange
		{
			add => _service.HourChangeEvent += value;
			remove => _service.HourChangeEvent -= value;
		}

		private IGameDayTimeService _service;

		public void Construct(IGameDayTimeService dayTimeService)
		{
			_service = dayTimeService;
			//_volume.profile.TryGet(out _colorAdjustments);
			OnSunrise += () => Debug.Log("Sunrise");
			OnSunset += () => Debug.Log("Sunset");
			OnHourChange += () => Debug.LogFormat("Hour change: {0}", _service.CurrentTime);
		}

		private void Update()
		{
			RotateSun();
			UpdateLightSettings();
			UpdateSkyBlend();

#if CHEAT_ENABLED
			if (Input.GetKeyDown(KeyCode.RightBracket))
			{
				_service.TimeMultiplier *= 2;
			}
			if (Input.GetKeyDown(KeyCode.LeftBracket))
			{
				_service.TimeMultiplier /= 2;
			}
#endif
		}

		private void UpdateSkyBlend()
		{
			var dotProduct = Vector3.Dot(_sun.transform.forward, Vector3.up);
			var blend = Mathf.Lerp(
				0,
				1,
				_lightIntensityCurve.Evaluate(dotProduct));
			_skyboxMaterial.SetFloat("_Blend", blend);
		}

		private void UpdateLightSettings()
		{
			var dotProduct = Vector3.Dot(_sun.transform.forward, Vector3.down);
			var lightIntensity = _lightIntensityCurve.Evaluate(dotProduct);

			_sun.intensity = Mathf.Lerp(
				0,
				_maxSunIntensity,
				lightIntensity);
			_moon.intensity = Mathf.Lerp(
				_maxMoonIntensity,
				0,
				lightIntensity);

			// if (_colorAdjustments == null)
			// 	return;
			// _colorAdjustments.colorFilter.value = Color.Lerp(
			// 	_nightAmbientLight,
			// 	_dayAmbientLight,
			// 	lightIntensity);
		}

		private void RotateSun()
		{
			var rotationSun = _service.CalculateSunAngle();
			_sun.transform.rotation = Quaternion.AngleAxis(rotationSun, _axisRotation);
			var rotationMoon = _service.CalculateSunAngle();
			_moon.transform.rotation = Quaternion.AngleAxis(rotationMoon + 180, _axisRotation);
		}
	}
}