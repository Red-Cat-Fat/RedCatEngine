using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;
using RedCatEngine.Windows.Interfaces;
using UnityEngine;

namespace RedCatEngine.Windows.Components.Windows
{
	public abstract class BaseWindowConfig : BaseConfig
	{
		public WindowLayer Layer
			=> _layer;
		public GameObject WindowPrefab
			=> _windowPrefab;

		[SerializeField]
		protected GameObject _windowPrefab;
		[SerializeField]
		private WindowLayer _layer;
		public abstract IModel GetModel();
		public abstract IWindowData MakeWindowData(ICreator windowContainer, params object[] context);
	}
}