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

	readonly ScreenStack _screenStack = new ScreenStack();
	bool _showMenu = true;

	UIDocument _uiDocument;
	UIScreen _mainScreen;
	UIScreen _playScreen;
	UIScreen _optionsScreen;

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
		_mainScreen = new UIScreen(_uiDocument.rootVisualElement.Q<VisualElement>("MainScreen"));
		_playScreen = new UIScreen(_uiDocument.rootVisualElement.Q<VisualElement>("PlayScreen"));
		_optionsScreen = new UIScreen(_uiDocument.rootVisualElement.Q<VisualElement>("OptionsScreen"));
		_screenStack.PushScreen(_mainScreen);

		_playButton = _mainScreen.Root.Q<Button>("PlayButton_Button");
		_continueButton = _mainScreen.Root.Q<Button>("ContinueButton_Button");
		_optionsButton = _mainScreen.Root.Q<Button>("OptionsButton_Button");
		_menuButton = _mainScreen.Root.Q<Button>("MenuButton_Button");
		_quitButton = _mainScreen.Root.Q<Button>("QuitButton_Button");

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
		_screenStack.ClearStack();
		if (newContext == MenuContext.MainMenu)
		{
			_mainScreen.Root.RemoveFromClassList("InGame");
		}
		else if (newContext == MenuContext.InGameMenu)
		{
			_mainScreen.Root.AddToClassList("InGame");
			ToggleMenu(false);
		}
	}

	void ToggleMenuKeyboard(InputAction.CallbackContext _)
	{
		if (_showMenu && !_screenStack.IsEmpty)
		{
			_screenStack.PopScreen();
			return;
		}

		if (CurrentContext == MenuContext.InGameMenu)
		{
			ToggleMenu(!_showMenu);
		}
	}

	void ToggleMenu(bool showMenu)
	{
		_screenStack.ClearStack();
		_showMenu = showMenu;
		_uiDocument.rootVisualElement.style.display = _showMenu ? DisplayStyle.Flex : DisplayStyle.None;
		if (CurrentContext == MenuContext.InGameMenu)
		{
			Time.timeScale = _showMenu ? 0 : 1;
			FindFirstObjectByType<CameraController>().enabled = !_showMenu;
		}
	}

	void Play() => _screenStack.PushScreen(_playScreen);

	void Continue() => ToggleMenu(false);

	void Options() => _screenStack.PushScreen(_optionsScreen);

	void ToMainMenu() => SceneManager.LoadScene("Menu");

	void Quit() => Application.Quit();
}
