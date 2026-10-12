using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Application;
using RedCatEngine.DependencyInjection.Specials.CreatedRule;

namespace RedCatEngine.DependencyInjection.Tests
{
	/// <summary>
	/// ТК2 (Х1, И2): базовый (не интерфейсный) тип <see cref="ContainerModifyRule"/> объявлен в
	/// сборке <c>DependencyInjection</c>, а наследник <see cref="CrossAssemblyModifyRuleSubclass"/>
	/// — в сборке <c>DependencyInjectionTests</c> (см. <c>Tests.asmdef</c> этой папки). До фикса
	/// <c>CashContainer.GetChildTypes</c> для конкретных типов сканировал
	/// <c>Assembly.GetAssembly(type).GetTypes()</c> — только сборку самого базового типа — и такого
	/// наследника не находил. На старой реализации оба теста этого класса воспроизводимо падали.
	/// </summary>
	public sealed class CashContainerConcreteBaseResolveTests
	{
		private IApplicationContainer _applicationContainer;

		[SetUp]
		public void SetUp()
		{
			_applicationContainer = new ApplicationContainer();
		}

		[Test]
		public void
				GivenSubclassFromAnotherAssemblyBoundByOwnType_WhenResolvingProductionBaseTypeViaTryGetSingle_ThenSubclassInstanceIsFound()
		{
			var subclassInstance = new CrossAssemblyModifyRuleSubclass();
			_applicationContainer.BindAsSingle(subclassInstance);

			var resolved = _applicationContainer.TryGetSingle<ContainerModifyRule>(out var data);

			Assert.IsTrue(resolved, "Наследник из другой сборки не найден по базовому типу");
			Assert.AreSame(subclassInstance, data);
		}

		[Test]
		public void
				GivenSubclassFromAnotherAssemblyBoundByOwnType_WhenResolvingProductionBaseTypeViaGetSingle_ThenSubclassInstanceIsFound()
		{
			var subclassInstance = new CrossAssemblyModifyRuleSubclass();
			_applicationContainer.BindAsSingle(subclassInstance);

			var resolved = _applicationContainer.GetSingle<ContainerModifyRule>();

			Assert.AreSame(subclassInstance, resolved);
		}
	}

	/// <summary>
	/// Тестовая заглушка, намеренно объявленная в сборке <c>DependencyInjectionTests</c>, а не
	/// рядом с <see cref="ContainerModifyRule"/> в сборке <c>DependencyInjection</c> — честное
	/// воспроизведение сценария Х1, где наследник живёт не в той сборке, что его конкретный
	/// (не интерфейсный) базовый тип.
	/// </summary>
	public sealed class CrossAssemblyModifyRuleSubclass : ContainerModifyRule
	{
	}
}
