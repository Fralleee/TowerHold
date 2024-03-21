using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
	public static string SceneToLoad;
	[SerializeField] Image _progressBarImage;

	void Start()
	{
		if (_progressBarImage != null)
		{
			_progressBarImage.fillAmount = 0;
		}

		_ = StartCoroutine(LoadSceneAsync(SceneToLoad));
	}

	IEnumerator LoadSceneAsync(string sceneName)
	{
		var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		asyncLoad.allowSceneActivation = false;

		while (!asyncLoad.isDone)
		{
			var progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Calculate progress
			_progressBarImage.fillAmount = progress; // Update the progress bar

			// Check if the load has finished
			if (asyncLoad.progress >= 0.9f)
			{
				asyncLoad.allowSceneActivation = true; // Allow scene activation
			}

			yield return null;
		}
	}
}
