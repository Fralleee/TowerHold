using UnityEngine;
using UnityEngine.UIElements;

public class ShopUI : MonoBehaviour
{
	Button _button;
	VisualElement _inventory;

	bool _showInventory;

	void OnEnable()
	{
		var uiDocument = GetComponent<UIDocument>();

		_button = uiDocument.rootVisualElement.Q("ToggleButton") as Button;
		_button.RegisterCallback<ClickEvent>(ToggleShop);

		_inventory = uiDocument.rootVisualElement.Q("Inventory");
	}


	void ToggleShop(ClickEvent evt)
	{
		_showInventory = !_showInventory;

		if (_showInventory)
		{
			_inventory.AddToClassList("active");
			_button.AddToClassList("active");
		}
		else
		{
			_inventory.RemoveFromClassList("active");
			_button.RemoveFromClassList("active");
		}
	}

	void OnDisable()
	{
		_button.UnregisterCallback<ClickEvent>(ToggleShop);
	}
}
