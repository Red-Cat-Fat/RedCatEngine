using System;
using Infrastructure.Windows.Interfaces;
using RedCatEngine.Configs;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;
using UnityEngine;

namespace Infrastructure.Windows.Components.Windows
{
	public abstract class WindowConfig : BaseConfig
	{
		[SerializeField] private WindowConfig _parent;
		[SerializeField] protected GameObject _windowPrefab;
		[SerializeField] private WindowLayer _layer;

		public WindowLayer Layer
			=> _layer;

		public GameObject WindowPrefab
			=> _windowPrefab;

		public WindowConfig Parent 
			=> _parent;

		public abstract Type ModelType { get; }

		public abstract IWindowData MakeWindowData(ICreator windowContainer, params object[] context);
	}
}