using System;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;

namespace RedCatEngine.DependencyInjection.Specials.CreatedRule
{
	public class UnityContainerModifyRule : ContainerModifyRule
	{
		protected override void DoCustomBind(IApplicationContainer container)
		{
			if (container is IUnityGameContainer unityContainer)
				container.BindAsSingle(unityContainer);
		}

		public new UnityContainerModifyRule Add(Action<IApplicationContainer> action)
		{
			AddAction(action);
			return this;
		}

		public UnityContainerModifyRule Add(UnityContainerModifyRule action)
		{
			base.Add(action);
			return this;
		}

		public UnityContainerModifyRule BindType<TBindType, TInstanceType>(params object[] context)
			where TInstanceType : TBindType
		{
			return Add(container => container.BindType<TBindType, TInstanceType>(context));
		}

		public UnityContainerModifyRule BindType<TInstanceType>(params object[] context)
		{
			return Add(container => container.BindType<TInstanceType>(context));
		}
	}
}