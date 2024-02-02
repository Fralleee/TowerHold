using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopUI : Controller
{
	[SerializeField] ShopInventory _inventory;

	Button _toggleButton;
	Button _refreshButton;
	Button _lockButton;
	List<Button> _shopSlots;
	List<ShopItem> _shopItems;
	VisualElement _inventoryContainer;

	bool _showInventory;
	bool _lockInventory;

	int _refreshCost = 50;
	readonly int _refreshCostIncrement = 50;
	readonly int _shopTypeCount = Enum.GetValues(typeof(ShopType)).Length;

	RandomGenerator _randomGenerator;
	readonly WaitForSeconds _nextFrame = new WaitForSeconds(0.1f);

	readonly Dictionary<Button, Action> _buttonActions = new Dictionary<Button, Action>();


	protected override void Awake()
	{
		base.Awake();

		_randomGenerator = new RandomGenerator(GameController.Instance.StartSeed);
	}

	void OnEnable()
	{
		var uiDocument = GetComponent<UIDocument>();

		_toggleButton = uiDocument.rootVisualElement.Q("ToggleButton") as Button;
		_refreshButton = uiDocument.rootVisualElement.Q("RefreshButton") as Button;
		_lockButton = uiDocument.rootVisualElement.Q("LockButton") as Button;

		_inventoryContainer = uiDocument.rootVisualElement.Q("Inventory");
		_shopSlots = _inventoryContainer.Query<Button>(className: "ShopItem").ToList();
		_shopItems = new List<ShopItem>();

		_toggleButton.clicked += ToggleShop;
		_refreshButton.clicked += ManualRefresh;
		_lockButton.clicked += ToggleLock;

		for (var i = 0; i < _shopSlots.Count; i++)
		{
			var index = i;
			var slot = _shopSlots[index];
			void action()
			{ PurchaseItem(index); }

			slot.clicked += action;
			_buttonActions[slot] = action;
		}

		Controls.Keyboard.ToggleShop.performed += ctx => ButtonClicked(_toggleButton);
		Controls.Keyboard.RefreshShop.performed += ctx => ButtonClicked(_refreshButton);
		Controls.Keyboard.LockShop.performed += ctx => ButtonClicked(_lockButton);
		Controls.Keyboard.PurchaseItem.performed += ctx => PurchaseItemKey(ctx.control.name);

		GameController.OnLevelChanged += RefreshShop;
	}

	void RefreshShop()
	{
		_shopItems.Clear();
		foreach (var slot in _shopSlots)
		{
			var item = GetRandomItem(GameController.Instance.CurrentLevel);
			slot.SetEnabled(false);
			slot.RemoveFromClassList("itemized");
			SetupSlot(slot, item);
			_shopItems.Add(item);
		}

		StartCoroutine(PerformRefresh());
	}

	IEnumerator PerformRefresh()
	{
		yield return _nextFrame;
		foreach (var slot in _shopSlots)
		{
			slot.AddToClassList("itemized");
			slot.SetEnabled(true);
		}
	}

	void SetupSlot(Button slot, ShopItem item)
	{
		slot.style.backgroundImage = item.Image.texture;
		slot.style.backgroundColor = UIManager.Instance.GetShopItemColor(item);
		slot.SetEnabled(true);
	}

	void PurchaseItem(int index)
	{
		var button = _shopSlots[index];
		var item = _shopItems[index];

		if (ResourceManager.Instance.SpendResources(item.Cost))
		{
			item.OnPurchase();
			button.SetEnabled(false);
		}
	}

	ShopItem GetRandomItem(int currentLevel)
	{
		// Step 1: Group items by broader categories
		var groupedItems = new Dictionary<ShopType, List<ShopItem>>();
		foreach (var item in _inventory.Items)
		{
			if (item.MinLevel <= currentLevel)
			{
				if (!groupedItems.ContainsKey(item.ShopType))
				{
					groupedItems[item.ShopType] = new List<ShopItem>();
				}
				groupedItems[item.ShopType].Add(item);
			}
		}

		// Step 2: Check if there are eligible items
		if (groupedItems.Count == 0)
		{
			return null;
		}

		// Step 3: Randomly select a category
		var randomCategory = (ShopType)_randomGenerator.Next(0, _shopTypeCount);
		while (!groupedItems.ContainsKey(randomCategory) || groupedItems[randomCategory].Count == 0)
		{
			randomCategory = (ShopType)_randomGenerator.Next(0, _shopTypeCount);
		}

		// Step 4: Choose a random item from the selected category
		var itemsInCategory = groupedItems[randomCategory];
		var randomIndex = _randomGenerator.Next(0, itemsInCategory.Count);
		return itemsInCategory[randomIndex];
	}

	void ManualRefresh()
	{
		if (ResourceManager.Instance.SpendResources(_refreshCost))
		{
			RefreshShop();
			_refreshCost += _refreshCostIncrement;
		}
	}

	void ToggleShop()
	{
		_showInventory = !_showInventory;

		if (_showInventory)
		{
			_inventoryContainer.AddToClassList("active");
			_toggleButton.AddToClassList("active");
		}
		else
		{
			_inventoryContainer.RemoveFromClassList("active");
			_toggleButton.RemoveFromClassList("active");
		}
	}

	void ToggleLock()
	{
		_lockInventory = !_lockInventory;

		if (_lockInventory)
		{
			_lockButton.AddToClassList("active");
		}
		else
		{
			_lockButton.RemoveFromClassList("active");
		}
	}

	void ButtonClicked(Button target)
	{
		using var navigationSubmitEvent = new NavigationSubmitEvent() { target = target };
		target.SendEvent(navigationSubmitEvent);
	}


	void PurchaseItemKey(string itemString)
	{
		if (int.TryParse(itemString, out var itemKey))
		{
			itemKey -= 1;
			if (itemKey >= 0 && itemKey < _shopSlots.Count)
			{
				ButtonClicked(_shopSlots[itemKey]);
			}
		}
	}

	void OnDisable()
	{
		_toggleButton.clicked -= ToggleShop;
		_refreshButton.clicked -= ManualRefresh;
		_lockButton.clicked -= ToggleLock;

		foreach (var kvp in _buttonActions)
		{
			kvp.Key.clicked -= kvp.Value;
		}
		_buttonActions.Clear();

		Controls.Keyboard.ToggleShop.performed -= ctx => ButtonClicked(_toggleButton);
		Controls.Keyboard.RefreshShop.performed -= ctx => ButtonClicked(_refreshButton);
		Controls.Keyboard.LockShop.performed -= ctx => ButtonClicked(_lockButton);
		Controls.Keyboard.PurchaseItem.performed -= ctx => PurchaseItemKey(ctx.control.name);

		GameController.OnLevelChanged -= RefreshShop;
	}
}
