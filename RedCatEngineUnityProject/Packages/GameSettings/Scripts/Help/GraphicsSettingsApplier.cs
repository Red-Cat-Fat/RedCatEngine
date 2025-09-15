using UnityEngine;

namespace RedCatEngine.GameSettings.Help
{
	/// <summary>
	/// Класс для настройки графических параметров игры (разрешение, качество, FPS и т.д.)
	/// </summary>
	internal static class GraphicsSettingsApplier
	{
		/// <summary>
		/// Установка разрешения экрана и режима окна.
		/// </summary>
		/// <param name="width">Ширина экрана в пикселях.</param>
		/// <param name="height">Высота экрана в пикселях.</param>
		/// <param name="mode">Режим отображения: FullScreen, Windowed, MaximizedWindow и др.</param>
		/// <param name="refreshRate">Частота обновления экрана (в Гц).</param>
		public static void SetResolution(
			int width,
			int height,
			FullScreenMode mode,
			RefreshRate refreshRate
		)
		{
			Screen.SetResolution(
				width,
				height,
				mode,
				refreshRate);
		}

		/// <param name="level">Индекс уровня качества из списка Quality Settings.</param>
		/// <param name="applyExpensiveChanges">Применять ли дорогостоящие изменения (например, изменение теней).</param>
		public static void SetQualityLevel(int level, bool applyExpensiveChanges = true)
		{
			QualitySettings.SetQualityLevel(level, applyExpensiveChanges);
		}

		/// <summary>
		/// Включение или выключение отрисовки теней.
		/// </summary>
		/// <param name="enabled">Если true — включает тени, если false — отключает.</param>
		public static void SetShadows(bool enabled)
		{
			QualitySettings.shadows = enabled ? ShadowQuality.All : ShadowQuality.Disable;
		}

		/// <summary>
		/// Установка уровня антиалиасинга.
		/// </summary>
		/// <param name="level">Уровень MSAA: 0 — отключено, 2, 4, 8 — количество сэмплов.</param>
		public static void SetAntiAliasing(int level)
		{
			QualitySettings.antiAliasing = level;
		}

		/// <summary>
		/// Включение или выключение VSync (синхронизация вертикального сканирования).
		/// </summary>
		/// <param name="enabled">Если true — включает VSync, если false — отключает.</param>
		public static void SetVSync(bool enabled)
		{
			QualitySettings.vSyncCount = enabled ? 1 : 0;
		}

		/// <summary>
		/// Ограничение максимального количества кадров в секунду (FPS).
		/// </summary>
		/// <param name="targetFps">Целевое количество кадров в секунду. Если меньше 0 — ограничение отключается.</param>
		public static void SetFrameRateCap(int targetFps)
		{
			Application.targetFrameRate = targetFps > 0 
				? targetFps 
				: -1; // -1 = без лимита
		}

		/// <summary>
		/// Включение или выключение эффекта Motion Blur (движущегося размытия).
		/// </summary>
		/// <param name="enabled">Если true — включает Motion Blur, если false — отключает.</param>
		/// <remarks>
		/// Реализация зависит от используемой системы постобработки (URP/HDRP или PostProcessing Stack).
		/// Обычно требуется получить VolumeProfile и включить/выключить соответствующий компонент.
		/// </remarks>
		public static void SetMotionBlur(bool enabled)
		{
			// Здесь зависит от твоего постпроцессинга (URP/HDRP или PostProcessing Stack)
			// Обычно ищешь VolumeProfile и включаешь/выключаешь компонент
		}
	}
}
