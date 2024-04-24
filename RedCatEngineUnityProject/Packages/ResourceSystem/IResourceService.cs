using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RedCatEngine.Resources
{
	public interface IResourceService
	{
		void Initialize();
		Task<T> Load<T>(AssetReference assetReference) where T : class;
		Task<T> Load<T>(string address) where T : class;
		Task<GameObject> Instantiate(string address, Vector3 at);
		Task<GameObject> Instantiate(string address, Transform under);
		Task<GameObject> Instantiate(string address);
		void Cleanup();
	}
}