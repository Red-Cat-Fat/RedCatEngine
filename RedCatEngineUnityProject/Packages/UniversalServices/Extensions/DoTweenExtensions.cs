#if DOTWEEN
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace RedCatEngine.CommonServices.Extensions
{
	public static class DoTweenExtensions
	{
		public static TweenerCore<Vector3, Vector3, VectorOptions> DoFollow(
			Transform followTransform,
			Transform targetTransform,
			float duration
		)
			=> DOTween.To(
				() => followTransform.transform.position -
				      targetTransform.position,
				x => followTransform.transform.position =
					x + targetTransform.position,
				Vector2.zero,
				duration);

		public static TweenerCore<Vector3, Vector3, VectorOptions> DoFollow(
			Transform followTransform,
			Transform targetTransform,
			Vector3 deltaPosition,
			float duration
		)
			=> DOTween.To(
				() => followTransform.transform.position -
				      targetTransform.position,
				x => followTransform.transform.position =
					x + targetTransform.position,
				deltaPosition,
				duration);
	}
}
#endif
