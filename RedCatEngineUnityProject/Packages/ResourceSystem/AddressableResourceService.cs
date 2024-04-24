using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RedCatEngine.Resources
{
	public class AddressableResourceService : IResourceService
	{
		private readonly Dictionary<string, AsyncOperationHandle> _completedCash = new();
		private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();

		public void Initialize()
		{
			Addressables.InitializeAsync();
		}

		public async Task<T> Load<T>(AssetReference assetReference) where T : class
		{
			if (_completedCash.TryGetValue(assetReference.AssetGUID, out var completedHandle))
				return completedHandle.Result as T;

			return await RunWithCacheOnComplete(
				Addressables.LoadAssetAsync<T>(assetReference),
				cacheKey: assetReference.AssetGUID);
		}

		public async Task<T> Load<T>(string address) where T : class
		{
			if (_completedCash.TryGetValue(address, out var completedHandle))
				return completedHandle.Result as T;

			var handle = Addressables.LoadAssetAsync<T>(address);

			return await RunWithCacheOnComplete(
				handle,
				cacheKey: address);
		}

		public Task<GameObject> Instantiate(string address, Vector3 at) =>
			Addressables.InstantiateAsync(address,
				at,
				Quaternion.identity).Task;

		public Task<GameObject> Instantiate(string address, Transform under) =>
			Addressables.InstantiateAsync(address, under).Task;

		public Task<GameObject> Instantiate(string address) =>
			Addressables.InstantiateAsync(address).Task;

		public void Cleanup()
		{
			foreach (var resourceHandles in _handles.Values)
			foreach (var handle in resourceHandles)
				Addressables.Release(handle);

			_completedCash.Clear();
			_handles.Clear();
		}

		private async Task<T> RunWithCacheOnComplete<T>(AsyncOperationHandle<T> handle, string cacheKey) where T : class
		{
			handle.Completed += completeHandle => { _completedCash[cacheKey] = completeHandle; };

			AddHandle(cacheKey, handle);

			return await handle.Task;
		}

		private void AddHandle(string key, AsyncOperationHandle handle)
		{
			if (!_handles.TryGetValue(key, out var resourceHandles))
			{
				resourceHandles = new List<AsyncOperationHandle>();
				_handles[key] = resourceHandles;
			}

			resourceHandles.Add(handle);
		}
	}
}