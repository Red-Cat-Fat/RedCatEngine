using UnityEngine;

namespace Infrastructure.Windows.Interfaces
{
	public interface ILayerContainer
	{
		Transform GetParentLayer(WindowLayer layer);
	}
}