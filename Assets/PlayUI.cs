using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayUI : MonoBehaviour
{
	UIDocument _uiDocument;
	VisualElement _playScreen;
	Button _quickGameButton;

	void Awake()
	{
		_uiDocument = GetComponent<UIDocument>();
		_playScreen = _uiDocument.rootVisualElement.Q<VisualElement>("PlayScreen");

		_quickGameButton = _playScreen.Q<Button>("QuickButton_Button");

		_quickGameButton.clicked += Play;
	}

	void Play() => SceneManager.LoadScene("Game");

	void OnDestroy() => _quickGameButton.clicked -= Play;
}
