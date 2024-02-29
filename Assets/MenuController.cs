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
	VisualElement _root;

	Button _playButton;
	Button _continueButton;
	Button _menuButton;
	Button _quitButton;

	protected override void Awake()
	{
		base.Awake();

		_uiDocument = GetComponent<UIDocument>();
		_root = _uiDocument.rootVisualElement.Q<VisualElement>("Root");

		_playButton = _root.Q<Button>("PlayButton");
		_continueButton = _root.Q<Button>("ContinueButton");
		_menuButton = _root.Q<Button>("MenuButton");
		_quitButton = _root.Q<Button>("QuitButton");

		_playButton.clicked += Play;
		_continueButton.clicked += Continue;
		_menuButton.clicked += ToMainMenu;
		_quitButton.clicked += Quit;

		Controls.Keyboard.ToggleMenu.performed += ToggleMenuKeyboard;
	}

	void OnValidate()
	{
		_uiDocument = GetComponent<UIDocument>();
		if (_uiDocument.rootVisualElement == null)
		{
			return;
		}

		_root = _uiDocument.rootVisualElement.Q<VisualElement>("Root");
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

	protected override void OnDestroy()
	{
		base.OnDestroy();

		_playButton.clicked -= Play;
		_continueButton.clicked -= Continue;
		_menuButton.clicked -= ToMainMenu;
		_quitButton.clicked -= Quit;

		Controls.Keyboard.ToggleMenu.performed -= ToggleMenuKeyboard;
	}

	public void UpdateMenuContext(MenuContext newContext)
	{
		CurrentContext = newContext;
		if (newContext == MenuContext.MainMenu)
		{
			_root.RemoveFromClassList("InGame");
		}
		else if (newContext == MenuContext.InGameMenu)
		{
			_root.AddToClassList("InGame");
			ToggleMenu(false);
		}
	}

	void ToggleMenuKeyboard(InputAction.CallbackContext _)
	{
		if (CurrentContext == MenuContext.InGameMenu)
		{
			ToggleMenu(!_showMenu);
		}
	}

	void ToggleMenu(bool showMenu)
	{
		_showMenu = showMenu;
		_uiDocument.rootVisualElement.style.display = _showMenu ? DisplayStyle.Flex : DisplayStyle.None;
	}

	void Play() => SceneManager.LoadScene("Game");

	void Continue() => ToggleMenu(false);

	void ToMainMenu() => SceneManager.LoadScene("Menu");

	void Quit() => Application.Quit();
}

