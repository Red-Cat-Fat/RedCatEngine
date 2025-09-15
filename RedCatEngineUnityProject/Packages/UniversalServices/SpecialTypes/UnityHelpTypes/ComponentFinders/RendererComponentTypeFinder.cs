using UnityEngine;

namespace RedCatEngine.CommonServices.SpecialTypes.UnityHelpTypes.ComponentFinders
{
	public class RendererComponentTypeFinder<TBehaviour> : ComponentTypeFinder<TBehaviour> where TBehaviour : class
	{
		protected override bool IsValid(GameObject gameObject, TBehaviour component)
		{
			var renderer = gameObject.GetComponentInParent<Renderer>();
			if (renderer != null)
				return renderer.isVisible;
			renderer = gameObject.GetComponentInChildren<Renderer>();
			return renderer != null && renderer.isVisible;
		}
	}
}