using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ShopUI : Controller
{
	[SerializeField] ShopInventory _inventory;

	Button _toggleButton;
	Button _refreshButton;
	Button _lockButton;

	List<Button> _shopSlots;
	List<ShopItem> _shopItems;

	VisualElement _progressContainer;
	VisualElement _inventoryContainer;

	ItemInformation _itemInformation;

	CustomProgressBar _healthBar;
	CustomProgressBar _levelBar;

	bool _showInventory = true;
	bool _lockInventory;

	int _refreshCost = 5;
	readonly int _refreshCostIncrement = 1;
	readonly int _shopTypeCount = Enum.GetValues(typeof(ShopType)).Length;
	int? _currentlyHoveredItemIndex = null;


	RandomGenerator _randomGenerator;
	readonly WaitForSeconds _nextFrame = new WaitForSeconds(0.1f);

	readonly Dictionary<Button, Action> _buttonActions = new Dictionary<Button, Action>();

	readonly List<(VisualElement element, EventCallback<MouseEnterEvent> enterCallback, EventCallback<MouseLeaveEvent> leaveCallback)> _informationTargets = new List<(VisualElement, EventCallback<MouseEnterEvent>, EventCallback<MouseLeaveEvent>)>();


	protected override void Awake()
	{
		base.Awake();

		_randomGenerator = new RandomGenerator(GameController.Instance.StartSeed);

		var uiDocument = GetComponent<UIDocument>();

		_toggleButton = uiDocument.rootVisualElement.Q<Button>("ToggleButton");
		_refreshButton = uiDocument.rootVisualElement.Q<Button>("RefreshButton");
		_lockButton = uiDocument.rootVisualElement.Q<Button>("LockButton");

		_itemInformation = uiDocument.rootVisualElement.Q<ItemInformation>("ItemInformation");

		_progressContainer = uiDocument.rootVisualElement.Q("Progress");
		_healthBar = _progressContainer.Q<CustomProgressBar>("HealthBar");
		_levelBar = _progressContainer.Q<CustomProgressBar>("LevelBar");

		_inventoryContainer = uiDocument.rootVisualElement.Q("Inventory");
		_shopSlots = _inventoryContainer.Query<Button>(className: "ShopItem").ToList();
		for (var i = 0; i < _shopSlots.Count; i++)
		{
			var index = i;
			var slot = _shopSlots[index];
			EventCallback<MouseEnterEvent> enterCallback = e => OnMouseEnter(index);
			EventCallback<MouseLeaveEvent> leaveCallback = e => OnMouseLeave();
			slot.RegisterCallback(enterCallback);
			slot.RegisterCallback(leaveCallback);

			// Store the slot and its callbacks for later removal
			_informationTargets.Add((slot, enterCallback, leaveCallback));
		}

		_shopItems = new List<ShopItem>();

		_toggleButton.clicked += ToggleShop;
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

		Controls.Keyboard.ToggleShop.performed += ctx => ButtonClicked(_toggleButton);
		Controls.Keyboard.RefreshShop.performed += ctx => ButtonClicked(_refreshButton);
		Controls.Keyboard.LockShop.performed += ctx => ButtonClicked(_lockButton);
		Controls.Keyboard.PurchaseItem.performed += ctx => PurchaseItemKey(ctx.control.name);
		SetShortcutLabels();

		GameController.OnLevelChanged += RefreshShop;
		GameController.OnGameStart += OnGameStart;
		Tower.OnHealthChanged += OnHealthChanged;

	}

	void OnMouseEnter(int index)
	{
		_currentlyHoveredItemIndex = index;
		_itemInformation.AddToClassList("active");

		var item = _shopItems[index];
		_itemInformation.UpdateItemInformation(item);
	}

	void OnMouseLeave()
	{
		_currentlyHoveredItemIndex = null;
		_itemInformation.RemoveFromClassList("active");
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

		_ = StartCoroutine(PerformRefresh());
		if (_currentlyHoveredItemIndex.HasValue && _currentlyHoveredItemIndex.Value < _shopItems.Count)
		{
			var item = _shopItems[_currentlyHoveredItemIndex.Value];
			_itemInformation.UpdateItemInformation(item);
		}
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
		}
	}

	void ToggleShop()
	{
		_showInventory = !_showInventory;

		if (_showInventory)
		{
			_progressContainer.AddToClassList("active");
			_inventoryContainer.AddToClassList("active");
			_toggleButton.AddToClassList("active");
		}
		else
		{
			_progressContainer.RemoveFromClassList("active");
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

	void SetShortcutLabels()
	{
		_toggleButton.Q<Label>().text = Controls.Keyboard.ToggleShop.GetBindingDisplayString();
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
		GameController.OnGameStart -= OnGameStart;
		Tower.OnHealthChanged -= OnHealthChanged;
	}
}
