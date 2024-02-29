using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrapper : MonoBehaviour
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static void LoadMenu()
	{
		Addressables.LoadAssetAsync<GameObject>("Menu").Completed += handle =>
		{
			if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
			{
				var menuPrefab = handle.Result;
				_ = Instantiate(menuPrefab);
			}
		};
	}
}
