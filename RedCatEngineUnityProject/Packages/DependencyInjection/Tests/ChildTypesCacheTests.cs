using System.Linq;
using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Specials.CreatedRule;
using RedCatEngine.DependencyInjection.Specials.Diagnostics;

namespace RedCatEngine.DependencyInjection.Tests
{
	/// <summary>
	/// ТК4 (Х4, И9): повторный резолв одного и того же базового типа в одном доменном сеансе
	/// обязан возвращать тот же полный список наследников — <see cref="ChildTypesCache"/> не должен
	/// ни худеть, ни расширяться частично между вызовами.
	/// </summary>
	public sealed class ChildTypesCacheTests
	{
		private ChildTypesCache _cache;

		[SetUp]
		public void SetUp()
		{
			_cache = new ChildTypesCache(NullContainerDiagnostics.Instance);
		}

		[Test]
		public void GivenConcreteBaseType_WhenResolvedTwice_ThenSecondCallReturnsSameCachedInstance()
		{
			var first = _cache.GetChildTypes(typeof(ContainerModifyRule));
			var second = _cache.GetChildTypes(typeof(ContainerModifyRule));

			Assert.IsTrue(
					first.Contains(typeof(UnityContainerModifyRule)),
					"Известный наследник UnityContainerModifyRule не найден при первом резолве");
			Assert.AreSame(first, second, "Повторный резолв не переиспользовал закэшированный список");
		}

		[Test]
		public void GivenBaseType_WhenResolvingChildren_ThenBaseTypeItselfIsExcluded()
		{
			var children = _cache.GetChildTypes(typeof(ContainerModifyRule));

			Assert.IsFalse(
					children.Contains(typeof(ContainerModifyRule)),
					"Список наследников не должен включать сам базовый тип");
		}
	}
}
