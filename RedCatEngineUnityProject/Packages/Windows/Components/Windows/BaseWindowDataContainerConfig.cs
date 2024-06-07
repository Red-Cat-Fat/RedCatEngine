using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.Windows.Interfaces;
using UnityEngine;

namespace RedCatEngine.Windows.Components.Windows
{
	public abstract class BaseWindowDataContainerConfig : ScriptableObject, IWindowSettings
	{
		[SerializeField]
		protected GameObject Prefab;
		public abstract bool TryOpen(IModel model, IApplicationContainer applicationContainer);
		public uint ID { get; }
		public WindowLayer Layer { get; }
		public bool TryOpen(IApplicationContainer applicationContainer)
		{
			throw new System.NotImplementedException();
		}
	}
}