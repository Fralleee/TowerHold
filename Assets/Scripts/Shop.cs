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
	public ShopItem[] AvailableItems;

	ShopSlot[] _slots;
	int _refreshCost = 50;

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

	void Update() => ProgressBar.fillAmount = GameController.Instance.TimeLeft / GameController.Instance.LevelTime;

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
		// Filter out the items that meet the level requirement
		var eligibleItems = new List<ShopItem>();
		foreach (var item in AvailableItems)
		{
			if (item.MinLevel <= currentLevel)
			{
				eligibleItems.Add(item);
			}
		}

		// If there are no eligible items, return null or handle it as you prefer
		if (eligibleItems.Count == 0)
		{
			return null;
		}

		// Choose a random item from the eligible items
		var randomIndex = Random.Range(0, eligibleItems.Count);
		return eligibleItems[randomIndex];
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
