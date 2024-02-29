using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuController : SingletonController<MenuController>
{
	public enum MenuContext
	{
		MainMenu,
		InGameMenu
	}

	public MenuContext CurrentContext = MenuContext.MainMenu;

	bool _showMenu = true;

	UIDocument _uiDocument;
	VisualElement _mainPage;
	VisualElement _optionsPage;
	VisualElement _currentPage;

	Button _playButton;
	Button _continueButton;
	Button _optionsButton;
	Button _menuButton;
	Button _quitButton;

	protected override void Awake()
	{
		base.Awake();

		DontDestroyOnLoad(gameObject);

		_uiDocument = GetComponent<UIDocument>();
		_mainPage = _uiDocument.rootVisualElement.Q<VisualElement>("MainPage");
		_optionsPage = _uiDocument.rootVisualElement.Q<VisualElement>("OptionsPage");
		_currentPage = _mainPage;

		_playButton = _mainPage.Q<Button>("PlayButton");
		_continueButton = _mainPage.Q<Button>("ContinueButton");
		_optionsButton = _mainPage.Q<Button>("OptionsButton");
		_menuButton = _mainPage.Q<Button>("MenuButton");
		_quitButton = _mainPage.Q<Button>("QuitButton");

		_playButton.clicked += Play;
		_continueButton.clicked += Continue;
		_optionsButton.clicked += Options;
		_menuButton.clicked += ToMainMenu;
		_quitButton.clicked += Quit;

		Controls.Keyboard.ToggleMenu.performed += ToggleMenuKeyboard;

		SceneManager.activeSceneChanged += OnSceneChanged;

		if (SceneManager.GetActiveScene().name != "Menu")
		{
			UpdateMenuContext(MenuContext.InGameMenu);
		}
		else
		{
			UpdateMenuContext(MenuContext.MainMenu);
		}
	}

	void OnValidate()
	{
		_uiDocument = GetComponent<UIDocument>();
		if (_uiDocument.rootVisualElement == null)
		{
			return;
		}

		_mainPage = _uiDocument.rootVisualElement.Q<VisualElement>("MainPage");
		if (_showMenu)
		{
			_uiDocument.rootVisualElement.style.display = DisplayStyle.Flex;
		}
		else
		{
			_uiDocument.rootVisualElement.style.display = DisplayStyle.None;
		}

		UpdateMenuContext(CurrentContext);
	}


	void OnSceneChanged(Scene _, Scene next)
	{
		if (next.name != "Menu")
		{
			UpdateMenuContext(MenuContext.InGameMenu);
		}
		else
		{
			UpdateMenuContext(MenuContext.MainMenu);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		_playButton.clicked -= Play;
		_continueButton.clicked -= Continue;
		_menuButton.clicked -= ToMainMenu;
		_quitButton.clicked -= Quit;

		Controls.Keyboard.ToggleMenu.performed -= ToggleMenuKeyboard;

		SceneManager.activeSceneChanged -= OnSceneChanged;
	}

	public void UpdateMenuContext(MenuContext newContext)
	{
		CurrentContext = newContext;
		if (newContext == MenuContext.MainMenu)
		{
			_mainPage.RemoveFromClassList("InGame");
		}
		else if (newContext == MenuContext.InGameMenu)
		{
			_mainPage.AddToClassList("InGame");
			ToggleMenu(false);
		}
	}

	void ActivatePage(VisualElement page)
	{
		_currentPage.style.display = DisplayStyle.None;

		page.style.display = DisplayStyle.Flex;
		_currentPage = page;
	}

	void ToggleMenuKeyboard(InputAction.CallbackContext _)
	{
		ActivatePage(_mainPage);
		if (CurrentContext == MenuContext.InGameMenu)
		{
			ToggleMenu(!_showMenu);
		}
	}

	void ToggleMenu(bool showMenu)
	{
		_showMenu = showMenu;
		_uiDocument.rootVisualElement.style.display = _showMenu ? DisplayStyle.Flex : DisplayStyle.None;
		if (CurrentContext == MenuContext.InGameMenu)
		{
			Time.timeScale = _showMenu ? 0 : 1;
			FindFirstObjectByType<CameraController>().enabled = !_showMenu;
		}
	}

	void Play() => SceneManager.LoadScene("Game");

	void Continue() => ToggleMenu(false);

	void Options() => ActivatePage(_optionsPage);

	void ToMainMenu() => SceneManager.LoadScene("Menu");

	void Quit() => Application.Quit();
}
