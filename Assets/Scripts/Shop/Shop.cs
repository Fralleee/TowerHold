using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	public ShopSlot ShopSlotPrefab;
	public int NumberOfSlots = 9;
	public Transform Main;
	public Button RefreshButton;
	public Image ProgressBar;
	public TextMeshProUGUI RefreshCostText;
	public ShopInventory Inventory;

	ShopSlot[] _slots;
	int _refreshCost = 50;
	readonly int _shopTypeCount = System.Enum.GetValues(typeof(ShopType)).Length;

	void Awake()
	{
		_slots = new ShopSlot[NumberOfSlots];
		for (var i = 0; i < NumberOfSlots; i++)
		{
			_slots[i] = Instantiate(ShopSlotPrefab, Main);
		}
	}

	void Start()
	{
		RefreshShop();
		RefreshButton.onClick.AddListener(ManualRefresh);
		GameController.OnLevelChanged += RefreshShop;
	}

	void Update() => ProgressBar.fillAmount = GameController.Instance.TimeLeft / GameController.Instance.TimePerLevel;

	public void RefreshShop()
	{
		foreach (var slot in _slots)
		{
			var item = GetRandomItem(GameController.Instance.CurrentLevel);
			slot.SetupSlot(item);
		}

		RefreshCostText.text = _refreshCost.ToString();
	}

	ShopItem GetRandomItem(int currentLevel)
	{
		// Step 1: Group items by broader categories
		var groupedItems = new Dictionary<ShopType, List<ShopItem>>();
		foreach (var item in Inventory.Items)
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
		var randomCategory = (ShopType)RandomManager.Shop.Next(0, _shopTypeCount);
		while (!groupedItems.ContainsKey(randomCategory) || groupedItems[randomCategory].Count == 0)
		{
			randomCategory = (ShopType)RandomManager.Shop.Next(0, _shopTypeCount);
		}

		// Step 4: Choose a random item from the selected category
		var itemsInCategory = groupedItems[randomCategory];
		var randomIndex = RandomManager.Shop.Next(0, itemsInCategory.Count);
		return itemsInCategory[randomIndex];
	}


	void ManualRefresh()
	{
		if (ResourceManager.Instance.SpendResources(_refreshCost))
		{
			RefreshShop();
			_refreshCost += 50;
			RefreshCostText.text = _refreshCost.ToString();
		}
	}
}
