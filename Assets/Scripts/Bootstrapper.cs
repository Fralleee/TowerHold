using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrapper
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static void LoadMenu()
	{
		Addressables.LoadAssetAsync<GameObject>("Menu").Completed += handle =>
		{
			if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
			{
				var menuPrefab = handle.Result;
				_ = Object.Instantiate(menuPrefab);
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
				var audioManager = handle.Result;
				_ = Object.Instantiate(audioManager);
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
				var graphyPrefab = handle.Result;
				_ = Object.Instantiate(graphyPrefab);
			}
		};
	}
}
