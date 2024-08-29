using RedCatEngine.DependencyInjection.Containers.Attributes;

namespace RedCatEngine.DependencyInjection.Tests.SpecialSubClasses
{
	public class SimpleDemoDataParentClass
	{
		public readonly int Value;

		public SimpleDemoDataParentClass(int value)
		{
			Value = value;
		}

		public override bool Equals(object obj)
			=> obj is SimpleDemoDataParentClass other && other.GetType() == GetType() && Equals(other);

		private bool Equals(SimpleDemoDataParentClass other)
			=> Value == other.Value;

		public override int GetHashCode()
			=> Value;
	}

	public class SimpleDemoFirstDataChildClass : SimpleDemoDataParentClass
	{
		public SimpleDemoFirstDataChildClass(int value)
			: base(value) { }
	}

	public class SimpleDemoSecondDataChildClass : SimpleDemoDataParentClass
	{
		public SimpleDemoSecondDataChildClass(int value)
			: base(value) { }
	}

	public class SimpleDemoParentClass { }

	public class SimpleDemoChildFirstClass : SimpleDemoParentClass { }

	public class SimpleDemoChildSecondClass : SimpleDemoParentClass { }

	public class SimpleInjectedDemoParentClass
	{
		private readonly SimpleDemoFirstDataChildClass _demoFirstData;
		private readonly SimpleDemoSecondDataChildClass _demoSecondData;

		[Inject]
		public SimpleInjectedDemoParentClass(
			SimpleDemoFirstDataChildClass demoFirstData,
			SimpleDemoSecondDataChildClass demoSecondData
		)
		{
			_demoFirstData = demoFirstData;
			_demoSecondData = demoSecondData;
		}

		public bool ContainData()
		{
			return _demoFirstData != null && _demoSecondData != null;
		}

		public bool IsCorrect(int firstData, int secondData)
		{
			return _demoFirstData.Value == firstData && _demoSecondData.Value == secondData;
		}
	}
}