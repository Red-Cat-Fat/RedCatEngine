using RedCatEngine.Windows.Interfaces;

namespace RedCatEngine.Windows.Components.Windows
{
	public abstract class BaseModelWindowDataContainerConfig<TModel, TFactory> : BaseWindowDataContainerConfig where TModel : IModel
	{
		public bool TryOpen(IModel model, TFactory factory)
		{
			return model is TModel typedModel && DoOpen(typedModel);
		}

		protected abstract bool DoOpen(TModel typedModel);
	}
}