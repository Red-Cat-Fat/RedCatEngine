using System;
using System.Collections;
using Infrastructure.Windows.Interfaces;
using JetBrains.Annotations;
using RedCatEngine.DependencyInjection.Specials.Components;

namespace Infrastructure.Windows.Components
{
	public abstract class BaseView : MonoConstruct, IView
	{
		private bool _isOpen;
		public event Action ClickCloseEvent;

		public bool IsOpen
			=> _isOpen;

		public void Open()
		{
			_isOpen = true;
			gameObject.SetActive(true);
			DoOpen();

			StartCoroutine(OnNextFrameRender());
		}

		public void Close()
		{
			gameObject.SetActive(false);
			DoClose();
			_isOpen = false;
		}

		private IEnumerator OnNextFrameRender()
		{
			yield return null;
			DoAfterOpen();
		}

		/// <summary>
		/// Действия на следующий кадр после спавна префаба View. То есть после инициализации всех размеров и тд
		/// </summary>
		protected virtual void DoAfterOpen()
		{
		}

		/// <summary>
		/// Действия при спавне префаба View.
		/// Если есть механики, завязанные на размерах и/или позициях элементов,
		/// то стоит использовать <see cref="DoAfterOpen"/>,
		/// так как на данном этапе движок Unity
		/// ещё не успел инициализировать все элементы и привязать им размеры
		/// </summary>
		protected virtual void DoOpen()
		{
		}

		protected virtual void DoClose()
		{
		}

		[UsedImplicitly]
		public void ActionClose()
			=> ClickCloseEvent?.Invoke();
	}
}