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
			=> obj is SimpleDemoDataParentClass other 
			   && other.GetType() == GetType() 
			   && Equals(other);

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
}