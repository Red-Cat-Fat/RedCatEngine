using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RedCatEngine.DependencyInjection.Specials.Diagnostics;

namespace RedCatEngine.DependencyInjection.Containers
{
	/// <summary>
	/// Кэш «тип → все его наследники и реализации» с полным сканом загруженных сборок.
	/// Один экземпляр на дерево контейнеров: корневой <see cref="ApplicationContainer"/> создаёт кэш,
	/// дочерние берут родительский, поэтому сборки домена перечисляются один раз на весь кэш,
	/// а не на каждый запрошенный тип и не на каждый создаваемый контейнер.
	/// Плоский список отсортирован по <see cref="Type.AssemblyQualifiedName"/>: выбор реализации
	/// потребителями вроде <c>TryFindFirstChildByType</c> не должен зависеть от порядка загрузки
	/// сборок, который в редакторе и в плеере разный.
	/// Список — снимок домена на момент первого запроса: сборка, загруженная позже
	/// (<see cref="Assembly.Load(string)"/> и подобное), в кэш уже не попадёт.
	/// </summary>
	public sealed class ChildTypesCache
	{
		private readonly Dictionary<Type, Type[]> _cache = new();
		private readonly IContainerDiagnostics _diagnostics;

		private Type[] _domainTypes;

		public ChildTypesCache(IContainerDiagnostics diagnostics)
			=> _diagnostics = diagnostics ?? NullContainerDiagnostics.Instance;

		private Type[] DomainTypes
			=> _domainTypes ??= AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(GetLoadableTypes)
					.OrderBy(type => type.AssemblyQualifiedName, StringComparer.Ordinal)
					.ToArray();

		/// <summary>
		/// Наследники и реализации <paramref name="type"/> без самого типа. Одна проверка
		/// <see cref="Type.IsAssignableFrom"/> покрывает и интерфейс, и конкретный базовый класс:
		/// с отсечённым <c>candidate == type</c> она для классов совпадает с <c>IsSubclassOf</c>.
		/// </summary>
		public IReadOnlyList<Type> GetChildTypes(Type type)
		{
			if(_cache.TryGetValue(type, out var cached))
				return cached;

			var result = DomainTypes
					.Where(candidate => candidate != type && IsChildType(type, candidate))
					.ToArray();

			_cache[type] = result;

			return result;
		}

		/// <summary>
		/// Сборку, типы которой не загрузились, пропускает целиком. <c>exception.Types</c> здесь
		/// намеренно не используется: полуживые типы из частично загруженной сборки роняют
		/// нативную проверку наследования уже вне управляемого кода.
		/// </summary>
		private Type[] GetLoadableTypes(Assembly assembly)
		{
			if(assembly == null)
				return Array.Empty<Type>();

			try
			{
				return assembly.GetTypes();
			}
			catch(ReflectionTypeLoadException exception)
			{
				_diagnostics.Error(
						"Failed to load types from assembly {0}: {1}. LoaderExceptions: {2}",
						assembly.FullName,
						exception.Message,
						JoinLoaderExceptions(exception));
				return Array.Empty<Type>();
			}
			catch(TypeLoadException exception)
			{
				_diagnostics.Error(
						"TypeLoadException in assembly {0}: {1}",
						assembly.FullName,
						exception);
				return Array.Empty<Type>();
			}
		}

		private string JoinLoaderExceptions(ReflectionTypeLoadException exception)
		{
			var loaderExceptions = exception.LoaderExceptions;
			if(loaderExceptions == null
			   || loaderExceptions.Length == 0)
				return "<none>";

			return string.Join(
					"; ",
					loaderExceptions.Select(loaderException => loaderException == null ? "<null>" : loaderException.Message));
		}

		private bool IsChildType(Type type, Type candidate)
		{
			try
			{
				return type.IsAssignableFrom(candidate);
			}
			catch(TypeLoadException exception)
			{
				_diagnostics.Error(
						"TypeLoadException checking assignable {0} from {1}: {2}",
						type,
						candidate,
						exception);
				return false;
			}
		}
	}
}
