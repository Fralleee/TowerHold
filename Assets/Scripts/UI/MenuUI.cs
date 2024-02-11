using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuUI : Controller
{
	Button _playButton;
	// Button _optionsButton;
	Button _quitButton;

	protected override void Awake()
	{
		base.Awake();

		var uiDocument = GetComponent<UIDocument>();

		_playButton = uiDocument.rootVisualElement.Q<Button>("PlayButton");
		// _optionsButton = uiDocument.rootVisualElement.Q<Button>("OptionsButton");
		_quitButton = uiDocument.rootVisualElement.Q<Button>("QuitButton");

		_playButton.clicked += Play;
		// _optionsButton.clicked += ...;
		_quitButton.clicked += Quit;
	}

	public void Play() => SceneManager.LoadScene("Game");

	public void Quit() => Application.Quit();

	void OnDestroy()
	{
		_playButton.clicked -= Play;
		// _optionsButton.clicked -= ...;
		_quitButton.clicked -= Quit;
	}
}
