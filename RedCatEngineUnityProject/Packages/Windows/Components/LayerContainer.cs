using System;
using Infrastructure.Windows.Interfaces;
using UnityEngine;

namespace Infrastructure.Windows.Components
{
	public class LayerContainer : MonoBehaviour, ILayerContainer
	{
		[Serializable]
		public class TransformContainer
		{
			public WindowLayer Layer;
			public Transform Transform;
		}
		public TransformContainer[] TransformContainers;

		public Transform GetParentLayer(WindowLayer layer)
		{
			foreach (var transformContainer in TransformContainers)
			{
				if (transformContainer.Layer == layer)
				{
					return transformContainer.Transform;
				}
			}

			Debug.LogErrorFormat("Not found transform for {0} layer", layer);
			return null;
		}
	}
}