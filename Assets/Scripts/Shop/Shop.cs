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
	RandomGenerator _randomGenerator;
	int _refreshCost = 50;
	readonly int _shopTypeCount = System.Enum.GetValues(typeof(ShopType)).Length;

	void Awake()
	{
		_slots = new ShopSlot[NumberOfSlots];
		for (var i = 0; i < NumberOfSlots; i++)
		{
			_slots[i] = Instantiate(ShopSlotPrefab, Main);
		}

		Main.gameObject.SetActive(false);
	}

	void Start()
	{
		_randomGenerator = new RandomGenerator(GameController.Instance.StartSeed);

		RefreshButton.onClick.AddListener(ManualRefresh);
		RefreshCostText.text = _refreshCost.ToString();

		GameController.OnLevelChanged += RefreshShop;
	}

	void Update()
	{
		if (!GameController.Instance.GameHasStarted)
		{
			ProgressBar.fillAmount = GameController.Instance.FreezeTimeLeft / GameController.Instance.FreezeTime;
			ProgressBar.color = Color.yellow;
		}
		else
		{
			ProgressBar.fillAmount = GameController.Instance.TimeLeft / GameController.Instance.TimePerLevel;
			ProgressBar.color = Color.cyan;
		}
	}

	public void RefreshShop()
	{
		Main.gameObject.SetActive(true);
		foreach (var slot in _slots)
		{
			var item = GetRandomItem(GameController.Instance.CurrentLevel);
			slot.SetupSlot(item);
		}
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
			_refreshCost += 50;
			RefreshCostText.text = _refreshCost.ToString();
		}
	}
}
