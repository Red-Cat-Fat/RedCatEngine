using UnityEngine;

namespace RedCatEngine.Windows.Interfaces
{
	public interface ILayerContainer
	{
		Transform GetParentLayer(WindowLayer layer);
	}
}