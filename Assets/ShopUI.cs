using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShopUI : MonoBehaviour
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
	readonly int _shopTypeCount = System.Enum.GetValues(typeof(ShopType)).Length;

	RandomGenerator _randomGenerator;
	readonly WaitForSeconds _nextFrame = new WaitForSeconds(0.1f);

	void Awake()
	{
		_randomGenerator = new RandomGenerator(GameController.Instance.StartSeed);

		GameController.OnLevelChanged += RefreshShop;
	}

	void OnEnable()
	{
		var uiDocument = GetComponent<UIDocument>();

		_toggleButton = uiDocument.rootVisualElement.Q("ToggleButton") as Button;
		_toggleButton.RegisterCallback<ClickEvent>(ToggleShop);

		_refreshButton = uiDocument.rootVisualElement.Q("RefreshButton") as Button;
		_refreshButton.RegisterCallback<ClickEvent>(ManualRefresh);

		_lockButton = uiDocument.rootVisualElement.Q("LockButton") as Button;
		_lockButton.RegisterCallback<ClickEvent>(ToggleLock);

		_inventoryContainer = uiDocument.rootVisualElement.Q("Inventory");

		_shopSlots = _inventoryContainer.Query<Button>(className: "ShopItem").ToList();

		_shopItems = new List<ShopItem>();
	}

	void RefreshShop()
	{
		_shopItems.Clear();
		foreach (var slot in _shopSlots)
		{
			slot.RemoveFromClassList("itemized");
			var item = GetRandomItem(GameController.Instance.CurrentLevel);
			SetupSlot(slot, item);
		}

		StartCoroutine(PerformRefresh());
	}

	IEnumerator PerformRefresh()
	{
		yield return _nextFrame;
		foreach (var slot in _shopSlots)
		{
			slot.AddToClassList("itemized");
		}
	}

	void SetupSlot(Button slot, ShopItem item)
	{
		slot.UnregisterCallback<ClickEvent>(PurchaseItem);

		var color = UIManager.Instance.GetShopItemColor(item);
		slot.style.backgroundImage = item.Image.texture;
		slot.style.backgroundColor = color;

		slot.RegisterCallback<ClickEvent>(PurchaseItem);
		slot.SetEnabled(true);

		_shopItems.Add(item);
	}

	void PurchaseItem(ClickEvent clickEvent)
	{
		var clickedSlotIndex = _shopSlots.FindIndex(slot => slot == clickEvent.target);
		var item = _shopItems[clickedSlotIndex];

		if (ResourceManager.Instance.SpendResources(item.Cost))
		{
			item.OnPurchase();
			(clickEvent.target as Button).SetEnabled(false);
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

	void ManualRefresh(ClickEvent clickEvent)
	{
		if (ResourceManager.Instance.SpendResources(_refreshCost))
		{
			RefreshShop();
			_refreshCost += 50;
		}
	}

	void ToggleShop(ClickEvent clickEvent)
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

	void ToggleLock(ClickEvent clickEvent)
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

	void OnDisable()
	{
		_toggleButton.UnregisterCallback<ClickEvent>(ToggleShop);
	}
}
