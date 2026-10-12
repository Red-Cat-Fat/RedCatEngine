using System;
using System.Collections.Generic;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.Binders;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind;
using RedCatEngine.DependencyInjection.Specials.CreatedRule;
using RedCatEngine.DependencyInjection.Specials.Diagnostics;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Application
{
	public interface IApplicationContainer :
			IBinderApplicationContainer,
			ITypeBinderApplicationContainer,
			IGetterApplicationContainer,
			ICreator,
			IDisposable
	{
		public string Name { get; }

		/// <summary>
		/// Мост трассировки контейнера во внешнюю систему логирования; передаётся в конструктор
		/// корневого контейнера, дочерние контейнеры наследуют его от родителя при создании.
		/// Никогда не <c>null</c> — без явной передачи это <see cref="NullContainerDiagnostics.Instance"/>.
		/// </summary>
		public IContainerDiagnostics Diagnostics { get; }

		public IReadOnlyDictionary<Type, object> Objects { get; }
		public IReadOnlyList<IApplicationContainer> ChildContainers { get; }

		/// <summary>true после <see cref="IDisposable.Dispose"/> — различает «контейнер выгружен»
		/// от «ошибка при живом контейнере» для вызывающих, которым нельзя падать при выходе
		/// из сессии (например, отложенное восстановление мира).</summary>
		public bool IsDisposed { get; }

		IApplicationContainer CreateChildContainer(string name, ContainerModifyRule containerModifyRules);
	}
}
