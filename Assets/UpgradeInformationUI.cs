using System;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UpgradeInformationUI : Controller
{
	Box _attackTypesList;
	UpgradeInformation _incomeUpgrade;
	UpgradeInformation _defenseUpgrade;
	UpgradeInformation _normalUpgrade;
	UpgradeInformation _normalTurret;
	UpgradeInformation _siegeUpgrade;
	UpgradeInformation _siegeTurret;
	UpgradeInformation _technologyUpgrade;
	UpgradeInformation _technologyTurret;
	UpgradeInformation _arcaneUpgrade;
	UpgradeInformation _arcaneTurret;
	UpgradeInformation _voidUpgrade;
	UpgradeInformation _voidTurret;

	protected override void Awake()
	{
		base.Awake();
		var uiDocument = GetComponent<UIDocument>();
		_attackTypesList = uiDocument.rootVisualElement.Q<Box>("AttackTypesList");
		_incomeUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("income-upgrade-label"));
		_defenseUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("defense-upgrade-label"));
		_normalUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("normal-upgrade-label"));
		_normalTurret = new UpgradeInformation(_attackTypesList.Q<Label>("normal-turret-label"));
		_siegeUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("siege-upgrade-label"));
		_siegeTurret = new UpgradeInformation(_attackTypesList.Q<Label>("siege-turret-label"));
		_technologyUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("technology-upgrade-label"));
		_technologyTurret = new UpgradeInformation(_attackTypesList.Q<Label>("technology-turret-label"));
		_arcaneUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("arcane-upgrade-label"));
		_arcaneTurret = new UpgradeInformation(_attackTypesList.Q<Label>("arcane-turret-label"));
		_voidUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("void-upgrade-label"));
		_voidTurret = new UpgradeInformation(_attackTypesList.Q<Label>("void-turret-label"));

		Controls.Keyboard.ShowDetails.performed += ToggleDetails;
		Controls.Keyboard.ShowDetails.canceled += ToggleDetails;
		Tower.OnUpgrade += OnUpgrade;
	}

	void ToggleDetails(InputAction.CallbackContext context)
	{
		_attackTypesList.ToggleInClassList("show-details");
	}

	void OnUpgrade(ShopItem item)
	{
		var upgradeInformation = GetUpgradeInformation(item);
		upgradeInformation.IncrementNumber();
	}

	UpgradeInformation GetUpgradeInformation(ShopItem item)
	{
		return item.ShopType switch
		{
			ShopType.Income => _incomeUpgrade,
			ShopType.Defense => _defenseUpgrade,
			ShopType.Normal => item is Turret ? _normalTurret : _normalUpgrade,
			ShopType.Siege => item is Turret ? _siegeTurret : _siegeUpgrade,
			ShopType.Technology => item is Turret ? _technologyTurret : _technologyUpgrade,
			ShopType.Arcane => item is Turret ? _arcaneTurret : _arcaneUpgrade,
			ShopType.Void => item is Turret ? _voidTurret : _voidUpgrade,
			ShopType.Unspecified => throw new ArgumentOutOfRangeException(),
			_ => throw new ArgumentOutOfRangeException(),
		};
	}

	void OnDestroy()
	{
		Controls.Keyboard.ShowDetails.performed -= ToggleDetails;
		Controls.Keyboard.ShowDetails.canceled -= ToggleDetails;
		Tower.OnUpgrade -= OnUpgrade;
	}
}
