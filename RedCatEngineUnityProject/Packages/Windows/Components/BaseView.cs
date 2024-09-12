using System;
using JetBrains.Annotations;
using RedCatEngine.DependencyInjection.Specials.Components;
using RedCatEngine.Windows.Interfaces;

namespace RedCatEngine.Windows.Components
{
	public abstract class BaseView : MonoConstruct, IView
	{
		public event Action CloseEvent;

		public void Open()
		{
			gameObject.SetActive(true);
			DoOpen();
		}

		public void Close()
		{
			gameObject.SetActive(false);
			DoClose();
		}

		protected virtual void DoOpen()
		{
			
		}

		protected virtual void DoClose()
		{
			
		}
		[UsedImplicitly]
		public void ActionClose()
			=> CloseEvent?.Invoke();
	}
}