using System;
using System.Collections.Generic;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.Binders;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;

namespace RedCatEngine.DependencyInjection.Specials.CreatedRule
{
	public class ContainerModifyRule
	{
		private readonly List<Action<IApplicationContainer>> _actions = new();

		public ContainerModifyRule Add(Action<IApplicationContainer> action)
		{
			AddAction(action);
			return this;
		}

		protected void AddAction(Action<IApplicationContainer> action)
		{
			if (action != null)
				_actions.Add(action);
		}

		public void Build(IApplicationContainer container)
		{
			BindAsCreator(container);
			foreach (var action in _actions)
				action.Invoke(container);
		}

		private void BindAsCreator(IApplicationContainer container)
		{
			DoCustomBind(container);
			container.BindAsSingle<ICreator>(container);
			container.BindAsSingle<IBinderApplicationContainer>(container);
			container.BindAsSingle<ITypeBinderApplicationContainer>(container);
			container.BindAsSingle<IGetterApplicationContainer>(container);
		}

		public ContainerModifyRule Add(ContainerModifyRule rule)
		{
			_actions.AddRange(rule._actions);
			return this;
		}

		protected virtual void DoCustomBind(IApplicationContainer container)
		{
		}

		public TRule Cast<TRule>()
			where TRule : ContainerModifyRule
		{
			return (TRule)this;
		}
	}
}