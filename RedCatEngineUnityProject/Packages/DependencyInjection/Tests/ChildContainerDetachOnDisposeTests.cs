using System;
using NUnit.Framework;
using RedCatEngine.DependencyInjection.Containers;
using RedCatEngine.DependencyInjection.Containers.Attributes;
using RedCatEngine.DependencyInjection.Containers.Interfaces.Unity;
using RedCatEngine.DependencyInjection.Specials.CreatedRule;

namespace RedCatEngine.DependencyInjection.Tests
{
	/// <summary>
	/// Контейнеры рантайм-объектов (игроки, NPC) создаются и выгружаются многократно за сессию.
	/// Пока выгруженный ребёнок оставался в <see cref="IApplicationContainer.ChildContainers"/> родителя,
	/// вместе с ним удерживались все забинженные объекты и <see cref="IDisposable"/> — в том числе
	/// обёртки уничтоженных MonoBehaviour.
	/// </summary>
	public sealed class ChildContainerDetachOnDisposeTests
	{
		[Test]
		public void ChildDisposed_ParentNoLongerHoldsIt()
		{
			var parent = new ApplicationContainer();
			var child = parent.CreateChildContainer("Child", null);

			Assert.AreEqual(1, parent.ChildContainers.Count);

			child.Dispose();

			Assert.AreEqual(0, parent.ChildContainers.Count);
		}

		[Test]
		public void ChildDisposed_ParentKeepsOtherChildren()
		{
			var parent = new ApplicationContainer();
			var first = parent.CreateChildContainer("First", null);
			var second = parent.CreateChildContainer("Second", null);

			first.Dispose();

			Assert.AreEqual(1, parent.ChildContainers.Count);
			Assert.AreSame(second, parent.ChildContainers[0]);
			Assert.IsFalse(second.IsDisposed);
		}

		[Test]
		public void ChildrenCreatedAndDisposedRepeatedly_ParentListDoesNotGrow()
		{
			var parent = new ApplicationContainer();

			for(var index = 0; index < 50; index++)
				parent.CreateChildContainer($"Child[{index}]", null).Dispose();

			Assert.AreEqual(0, parent.ChildContainers.Count);
		}

		[Test]
		public void ParentDisposed_AllChildrenAreDisposedAndListIsEmpty()
		{
			var parent = new ApplicationContainer();
			var first = parent.CreateChildContainer("First", null);
			var second = parent.CreateChildContainer("Second", null);
			var third = parent.CreateChildContainer("Third", null);

			parent.Dispose();

			Assert.IsTrue(first.IsDisposed);
			Assert.IsTrue(second.IsDisposed);
			Assert.IsTrue(third.IsDisposed);
			Assert.AreEqual(0, parent.ChildContainers.Count);
		}

		[Test]
		public void ParentDisposed_DisposesContentOfEveryChildExactlyOnce()
		{
			var parent = new ApplicationContainer();
			var probes = new DisposableProbe[3];

			for(var index = 0; index < probes.Length; index++)
			{
				var child = parent.CreateChildContainer($"Child[{index}]", null);
				child.BindAsSingle($"Marker[{index}]");
				probes[index] = child.GetSingle<DisposableProbe>();
			}

			parent.Dispose();

			for(var index = 0; index < probes.Length; index++)
			{
				Assert.AreEqual($"Marker[{index}]", probes[index].Marker);
				Assert.AreEqual(1, probes[index].DisposeCount);
			}
		}

		[Test]
		public void ParentDisposed_AfterSomeChildrenAlreadyDisposed_DoesNotThrow()
		{
			var parent = new ApplicationContainer();
			var first = parent.CreateChildContainer("First", null);
			parent.CreateChildContainer("Second", null);
			var third = parent.CreateChildContainer("Third", null);

			first.Dispose();
			third.Dispose();

			Assert.DoesNotThrow(() => parent.Dispose());
			Assert.AreEqual(0, parent.ChildContainers.Count);
		}

		[Test]
		public void ChildDisposedTwice_DoesNotThrowAndParentStaysConsistent()
		{
			var parent = new ApplicationContainer();
			var child = parent.CreateChildContainer("Child", null);
			var sibling = parent.CreateChildContainer("Sibling", null);

			child.Dispose();

			Assert.DoesNotThrow(() => child.Dispose());
			Assert.AreEqual(1, parent.ChildContainers.Count);
			Assert.AreSame(sibling, parent.ChildContainers[0]);
		}

		[Test]
		public void GrandChildDisposed_OnlyDetachesFromItsOwnParent()
		{
			var root = new ApplicationContainer();
			var middle = root.CreateChildContainer("Middle", null);
			var leaf = middle.CreateChildContainer("Leaf", null);

			leaf.Dispose();

			Assert.AreEqual(0, middle.ChildContainers.Count);
			Assert.AreEqual(1, root.ChildContainers.Count);
			Assert.AreSame(middle, root.ChildContainers[0]);
		}

		[Test]
		public void UnityChildDisposed_ParentNoLongerHoldsIt()
		{
			IUnityGameContainer parent = new UnityGameContainer(new UnityContainerModifyRule());
			var child = parent.CreateChildContainer("Child", new UnityContainerModifyRule());

			Assert.AreEqual(1, parent.ChildContainers.Count);

			child.Dispose();

			Assert.AreEqual(0, parent.ChildContainers.Count);
		}

		/// <summary>
		/// Контейнер берёт на учёт для выгрузки только то, что создал сам по <see cref="InjectAttribute"/>-конструктору
		/// (<c>TryRegisterAsDisposable</c>), поэтому проба резолвится через <c>GetSingle</c>, а не биндится готовой.
		/// </summary>
		private sealed class DisposableProbe : IDisposable
		{
			[Inject]
			public DisposableProbe(string marker) => Marker = marker;

			public string Marker { get; }
			public int DisposeCount { get; private set; }

			public void Dispose() => DisposeCount++;
		}
	}
}
