using System;
using Infrastructure.Windows.Interfaces;
using RedCatEngine.CommonServices.Services.Logs;
using UnityEngine;

namespace Infrastructure.Windows.Components
{
	public class LayerContainer : MonoBehaviour, ILayerContainer
	{
		private static ILogService _log;
		private static int _logGeneration;
		public TransformContainer[] TransformContainers;

		private static ILogService Log
		{
			get
			{
				if(!LogHub.IsInitialized)
					return LogHub.Root;

				if(_log == null || _logGeneration != LogHub.Generation)
				{
					_log = LogHub.CreateTag("LayerContainer");
					_logGeneration = LogHub.Generation;
				}

				return _log;
			}
		}

		public Transform GetParentLayer(WindowLayer layer)
		{
			foreach(var transformContainer in TransformContainers)
			{
				if(transformContainer.Layer == layer)
				{
					return transformContainer.Transform;
				}
			}

			Log.LogErrorFormat("Not found transform for {0} layer", layer);
			return null;
		}

		[Serializable]
		public class TransformContainer
		{
			public WindowLayer Layer;
			public Transform Transform;
		}
	}
}
