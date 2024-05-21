using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ShopUI : Controller
{
	[SerializeField] ShopInventory _inventory;
	[SerializeField] FeedbackEvent _purchaseFeedback;

	TooltipController _tooltipController;
	TooltipContent _refreshTooltipContent;

	Button _refreshButton;
	Button _lockButton;

	List<ShopItemButton> _shopSlots;
	List<ShopItem> _shopItems;

	VisualElement _inventoryContainer;

	bool ShopReady => _shopSlots.Count == _shopItems.Count;
	bool _lockInventory;
	int _refreshCost = 50;
	readonly int _refreshCostIncrement = 50;

	RandomGenerator _randomGenerator;
	readonly WaitForSeconds _nextFrame = new WaitForSeconds(0.1f);

	readonly Dictionary<Button, Action> _buttonActions = new Dictionary<Button, Action>();

	EventBinding<GameStartEvent> _gameStartEvent;
	EventBinding<LevelChangedEvent> _levelChangedEvent;
	EventBinding<ResourceChangedEvent> _resourceChangedEvent;

	void OnEnable()
	{
		_gameStartEvent = new EventBinding<GameStartEvent>(e => _lockButton.SetEnabled(true));
		EventBus<GameStartEvent>.Register(_gameStartEvent);

		_levelChangedEvent = new EventBinding<LevelChangedEvent>(e => RefreshShop());
		EventBus<LevelChangedEvent>.Register(_levelChangedEvent);

		_resourceChangedEvent = new EventBinding<ResourceChangedEvent>(e => SetPurchasable());
		EventBus<ResourceChangedEvent>.Register(_resourceChangedEvent);
	}

	void OnDisable()
	{
		EventBus<GameStartEvent>.Deregister(_gameStartEvent);
		EventBus<LevelChangedEvent>.Deregister(_levelChangedEvent);
		EventBus<ResourceChangedEvent>.Deregister(_resourceChangedEvent);
	}

	protected override void Awake()
	{
		base.Awake();

		_randomGenerator = new RandomGenerator(GameController.GameSettings.StartSeed);
		_shopItems = new List<ShopItem>();

		var uiDocument = GetComponent<UIDocument>();
		_tooltipController = GetComponent<TooltipController>();

		_inventoryContainer = uiDocument.rootVisualElement.Q("Inventory");
		_inventoryContainer.AddToClassList("active");
		_refreshButton = uiDocument.rootVisualElement.Q<Button>("RefreshButton");
		_lockButton = uiDocument.rootVisualElement.Q<Button>("LockButton");
		_shopSlots = _inventoryContainer.Query<ShopItemButton>().ToList();

		_refreshTooltipContent = new TooltipContent(null, "Refresh", $"Cost: {_refreshCost}", "Refresh the shop to get new items");
		_tooltipController.RegisterTooltip(_refreshButton, _refreshTooltipContent);

		_refreshButton.clicked += ManualRefresh;
		_lockButton.clicked += ToggleLock;
		_lockButton.SetEnabled(false);
		_tooltipController.RegisterTooltip(_lockButton, new TooltipContent(null, "Lock", null, "Lock the shop"));

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
			_tooltipController.RegisterTooltip(slot, new TooltipContent(null, "Empty Slot", "This slot is currently empty."));
		}

		Controls.Keyboard.RefreshShop.performed += ctx => ButtonClicked(_refreshButton);
		Controls.Keyboard.LockShop.performed += ctx => ButtonClicked(_lockButton);
		Controls.Keyboard.PurchaseItem.performed += ctx => PurchaseItemKey(ctx.control.name);
		SetupHotkeyLabels();
	}

	void SetPurchasable()
	{
		if (!ShopReady)
		{
			return;
		}

		for (var i = 0; i < _shopSlots.Count; i++)
		{
			var item = _shopItems[i];
			item.CanBePurchased = item.Cost <= ResourceManager.Instance.Resources;

			var slot = _shopSlots[i];
			slot.SetCanBePurchased(item.CanBePurchased);
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
			_refreshTooltipContent.CostContainer.AddImageLabel(StyleManager.Styles.GetIcon(GameIcons.Gold), $"Cost: {_refreshCost}");
		}
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
			slot.Setup(item);
			_shopItems.Add(item);
		}

		SetPurchasable();
		UpdateShopItemTooltips();
		_ = StartCoroutine(PerformRefresh());
	}

	void UpdateShopItemTooltips()
	{
		for (var i = 0; i < _shopSlots.Count; i++)
		{
			var item = _shopItems[i];
			_tooltipController.UpdateTooltip(_shopSlots[i], item.Tooltip());
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

	void SetupHotkeyLabels()
	{
		_refreshButton.Q<Label>().text = Controls.Keyboard.RefreshShop.GetBindingDisplayString();
		_lockButton.Q<Label>().text = Controls.Keyboard.LockShop.GetBindingDisplayString();

		for (var i = 0; i < _shopSlots.Count; i++)
		{
			_shopSlots[i].SetHotkey(Controls.Keyboard.PurchaseItem.GetBindingDisplayString(i));
		}
	}

	void PurchaseItem(int index)
	{
		var button = _shopSlots[index];
		var item = _shopItems[index];

		if (ResourceManager.Instance.SpendResources(item.Cost))
		{
			item.OnPurchase();
			button.Disable();
			_purchaseFeedback.TriggerFeedback(Tower.Instance.transform, this);

			if (item is DamageUpgrade)
			{
				UpdateShopItemTooltips();
			}
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

	void ButtonClicked(Button target)
	{
		using var navigationSubmitEvent = new NavigationSubmitEvent() { target = target };
		target.SendEvent(navigationSubmitEvent);
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
	}
}
