using System;

namespace RedCatEngine.DependencyInjection.Containers.Interfaces.Application.GenerationBind
{
	public interface ITypeBinderApplicationContainer
	{
		TInstanceBindType BindDummy<TInstanceBindType, TDummyType>(params object[] context)
			where TDummyType : TInstanceBindType;

		TBindType BindType<TBindType, TInstanceType>(params object[] context)
			where TInstanceType : TBindType;
		TBindArrayType BindArrayType<TBindArrayType, TInstanceType>(params object[] context)
			where TInstanceType : TBindArrayType;

		TInstanceBindType BindType<TInstanceBindType>(params object[] context);
		object BindType(Type type, params object[] context);
		TInstanceBindType BindArrayType<TInstanceBindType>(params object[] context);
		object BindArrayType(Type type, params object[] context);
	}
}