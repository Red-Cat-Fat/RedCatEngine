using System;
using JetBrains.Annotations;

namespace RedCatEngine.DependencyInjection.Containers.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	[MeansImplicitUse]
	public class MonoInjectAttribute : Attribute
	{
		
	}
}