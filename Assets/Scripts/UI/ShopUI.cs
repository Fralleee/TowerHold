using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ShopUI : Controller
{
	[SerializeField] ShopInventory _inventory;

	Tooltip _tooltip;
	TooltipController _tooltipController;
	TooltipContent _refreshTooltipContent;

	Button _refreshButton;
	Button _lockButton;

	List<Button> _shopSlots;
	List<ShopItem> _shopItems;

	VisualElement _progressContainer;
	VisualElement _inventoryContainer;

	Label _levelLabel;
	Label _coinLabel;
	Label _incomeLabel;

	CustomProgressBar _healthBar;
	CustomProgressBar _levelBar;

	bool _lockInventory;

	int _refreshCost = 5;
	readonly int _refreshCostIncrement = 1;
	readonly int _shopTypeCount = Enum.GetValues(typeof(ShopType)).Length;

	RandomGenerator _randomGenerator;
	readonly WaitForSeconds _nextFrame = new WaitForSeconds(0.1f);

	readonly Dictionary<Button, Action> _buttonActions = new Dictionary<Button, Action>();

	protected override void Awake()
	{
		base.Awake();

		_randomGenerator = new RandomGenerator(GameController.Instance.StartSeed);

		var uiDocument = GetComponent<UIDocument>();
		_tooltipController = GetComponent<TooltipController>();

		_refreshButton = uiDocument.rootVisualElement.Q<Button>("RefreshButton");
		_lockButton = uiDocument.rootVisualElement.Q<Button>("LockButton");

		_levelLabel = uiDocument.rootVisualElement.Q<Label>("LevelLabel");
		_coinLabel = uiDocument.rootVisualElement.Q<Label>("CoinLabel");
		_incomeLabel = uiDocument.rootVisualElement.Q<Label>("IncomeLabel");

		_tooltip = uiDocument.rootVisualElement.Q<Tooltip>();

		_progressContainer = uiDocument.rootVisualElement.Q("Progress");
		_healthBar = _progressContainer.Q<CustomProgressBar>("HealthBar");
		_levelBar = _progressContainer.Q<CustomProgressBar>("LevelBar");

		_inventoryContainer = uiDocument.rootVisualElement.Q("Inventory");
		_shopSlots = _inventoryContainer.Query<Button>(className: "ShopItem").ToList();

		_refreshTooltipContent = new TooltipContent("Refresh", $"Cost: {_refreshCost}", "Refresh the shop to get new items");

		_tooltipController.RegisterTooltip(_refreshButton, _refreshTooltipContent);


		_shopItems = new List<ShopItem>();

		_refreshButton.clicked += ManualRefresh;
		_lockButton.clicked += ToggleLock;
		_lockButton.SetEnabled(false);

		for (var i = 0; i < _shopSlots.Count; i++)
		{
			var index = i;
			var slot = _shopSlots[index];
			void action()
			{ PurchaseItem(index); }

			slot.clicked += action;
			_buttonActions[slot] = action;
		}

		for (var i = 0; i < _shopSlots.Count; i++)
		{
			var index = i;
			var slot = _shopSlots[index];
			_tooltipController.RegisterTooltip(slot, new TooltipContent());
		}

		Controls.Keyboard.RefreshShop.performed += ctx => ButtonClicked(_refreshButton);
		Controls.Keyboard.LockShop.performed += ctx => ButtonClicked(_lockButton);
		Controls.Keyboard.PurchaseItem.performed += ctx => PurchaseItemKey(ctx.control.name);
		SetShortcutLabels();

		GameController.OnLevelChanged += OnLevelChanged;
		GameController.OnGameStart += OnGameStart;
		Tower.OnHealthChanged += OnHealthChanged;
		ResourceManager.OnResourceChange += OnResourceChanged;
		ResourceManager.OnIncomeChange += OnIncomeChanged;
	}

	TooltipContent GetSlotItem(int index)
	{
		var item = _shopItems[index];
		return item.Tooltip();
	}

	void Start()
	{
		_healthBar.UseChangeBar = true;
		_levelBar.UseChangeBar = false;
		InvokeRepeating(nameof(UpdateLevelProgress), 0, 1f);
	}

	void UpdateLevelProgress()
	{
		_levelBar.MinMaxValue = GameController.Instance.GameHasStarted
		? (Mathf.CeilToInt(GameController.Instance.TimeLeft), Mathf.CeilToInt(GameController.Instance.TimePerLevel))
		: (Mathf.CeilToInt(GameController.Instance.FreezeTimeLeft), Mathf.CeilToInt(GameController.Instance.FreezeTime));

		var value = Mathf.Round(GameController.Instance.LevelProgress * GameController.Instance.TimePerLevel) / GameController.Instance.TimePerLevel;
		_levelBar.Value = value;
	}

	void OnGameStart()
	{
		_levelBar.AddToClassList("active");
		_lockButton.SetEnabled(true);
	}

	void OnHealthChanged(int currentHealth, int maxHealth)
	{
		_healthBar.MinMaxValue = (currentHealth, maxHealth);
		_healthBar.Value = currentHealth / (float)maxHealth;
	}

	void OnLevelChanged(int level)
	{
		_levelLabel.text = $"Level: {level}";
		RefreshShop();
	}

	void OnResourceChanged(int coin)
	{
		_coinLabel.text = $"Coin: {coin}";
	}

	void OnIncomeChanged(int income)
	{
		_incomeLabel.text = $"Income: {income}";
	}

	void RefreshShop()
	{
		if (_lockInventory)
		{
			return;
		}

		_shopItems.Clear();
		foreach (var slot in _shopSlots)
		{
			var item = ShopItem.GetRandomItem(GameController.Instance.CurrentLevel, _inventory.Items, _randomGenerator);
			slot.SetEnabled(false);
			slot.RemoveFromClassList("itemized");
			SetupSlot(slot, item);
			_shopItems.Add(item);
		}

		for (var i = 0; i < _shopSlots.Count; i++)
		{
			var item = _shopItems[i];
			_tooltipController.UpdateTooltip(_shopSlots[i], GetSlotItem(i));
		}

		_ = StartCoroutine(PerformRefresh());
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

		var rarityClasses = new List<string> { "gray", "green", "blue", "purple", "orange" };
		foreach (var rarityClass in rarityClasses)
		{
			slot.RemoveFromClassList(rarityClass);
		}

		slot.AddToClassList(Rarity.AsColorClass(item.RarityType));

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

	void ManualRefresh()
	{
		if (_lockInventory)
		{
			ToggleLock();
		}

		if (ResourceManager.Instance.SpendResources(_refreshCost))
		{
			RefreshShop();
			_refreshCost += _refreshCostIncrement;
			_refreshTooltipContent.CostLabel.text = $"Cost: {_refreshCost}";
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

	void SetShortcutLabels()
	{
		_refreshButton.Q<Label>().text = Controls.Keyboard.RefreshShop.GetBindingDisplayString();
		_lockButton.Q<Label>().text = Controls.Keyboard.LockShop.GetBindingDisplayString();

		for (var i = 0; i < _shopSlots.Count; i++)
		{
			_shopSlots[i].Q<Label>().text = Controls.Keyboard.PurchaseItem.GetBindingDisplayString(i);
		}
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

	void OnDestroy()
	{
		_refreshButton.clicked -= ManualRefresh;
		_lockButton.clicked -= ToggleLock;

		foreach (var kvp in _buttonActions)
		{
			kvp.Key.clicked -= kvp.Value;
		}
		_buttonActions.Clear();

		Controls.Keyboard.RefreshShop.performed -= ctx => ButtonClicked(_refreshButton);
		Controls.Keyboard.LockShop.performed -= ctx => ButtonClicked(_lockButton);
		Controls.Keyboard.PurchaseItem.performed -= ctx => PurchaseItemKey(ctx.control.name);

		GameController.OnLevelChanged -= OnLevelChanged;
		GameController.OnGameStart -= OnGameStart;
		Tower.OnHealthChanged -= OnHealthChanged;
		ResourceManager.OnResourceChange -= OnResourceChanged;
	}
}
