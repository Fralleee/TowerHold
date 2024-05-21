using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrapper
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	static void LoadStyleManager()
	{
		Addressables.LoadAssetAsync<GameObject>("StyleManager").Completed += handle =>
		{
			if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
			{
				var prefab = handle.Result;
				_ = Object.Instantiate(prefab);
			}
		};
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static void LoadMenu()
	{
		Addressables.LoadAssetAsync<GameObject>("Menu").Completed += handle =>
		{
			if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
			{
				var prefab = handle.Result;
				_ = Object.Instantiate(prefab);
			}
		};
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static void LoadAudioManager()
	{
		Addressables.LoadAssetAsync<GameObject>("AudioManager").Completed += handle =>
		{
			if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
			{
				var prefab = handle.Result;
				_ = Object.Instantiate(prefab);
			}
		};
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static void LoadGraphy()
	{
		Addressables.LoadAssetAsync<GameObject>("Graphy").Completed += handle =>
		{
			if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
			{
				var prefab = handle.Result;
				_ = Object.Instantiate(prefab);
			}
		};
	}
}
