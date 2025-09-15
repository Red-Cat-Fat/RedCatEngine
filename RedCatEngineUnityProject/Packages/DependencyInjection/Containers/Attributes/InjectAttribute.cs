using System;
using JetBrains.Annotations;

namespace RedCatEngine.DependencyInjection.Containers.Attributes
{
	[AttributeUsage(AttributeTargets.Constructor)]
	[MeansImplicitUse]
	public class InjectAttribute : Attribute
	{
		
	}
}