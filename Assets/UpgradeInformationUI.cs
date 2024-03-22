using System;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UpgradeInformationUI : Controller
{
	Box _attackTypesList;
	UpgradeInformation _incomeUpgrade;
	UpgradeInformation _defenseUpgrade;
	UpgradeInformation _powerUpgrade;
	UpgradeInformation _powerTurret;
	UpgradeInformation _precisionUpgrade;
	UpgradeInformation _precisionTurret;
	UpgradeInformation _technologyUpgrade;
	UpgradeInformation _technologyTurret;
	UpgradeInformation _arcaneUpgrade;
	UpgradeInformation _arcaneTurret;
	UpgradeInformation _chemicalUpgrade;
	UpgradeInformation _chemicalTurret;

	protected override void Awake()
	{
		base.Awake();
		var uiDocument = GetComponent<UIDocument>();
		_attackTypesList = uiDocument.rootVisualElement.Q<Box>("AttackTypesList");
		_incomeUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("income-upgrade-label"));
		_defenseUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("defense-upgrade-label"));
		_powerUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("power-upgrade-label"));
		_powerTurret = new UpgradeInformation(_attackTypesList.Q<Label>("power-turret-label"));
		_precisionUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("precision-upgrade-label"));
		_precisionTurret = new UpgradeInformation(_attackTypesList.Q<Label>("precision-turret-label"));
		_technologyUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("technology-upgrade-label"));
		_technologyTurret = new UpgradeInformation(_attackTypesList.Q<Label>("technology-turret-label"));
		_arcaneUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("arcane-upgrade-label"));
		_arcaneTurret = new UpgradeInformation(_attackTypesList.Q<Label>("arcane-turret-label"));
		_chemicalUpgrade = new UpgradeInformation(_attackTypesList.Q<Label>("chemical-upgrade-label"));
		_chemicalTurret = new UpgradeInformation(_attackTypesList.Q<Label>("chemical-turret-label"));

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
			ShopType.Power => item is Turret ? _powerTurret : _powerUpgrade,
			ShopType.Precision => item is Turret ? _precisionTurret : _precisionUpgrade,
			ShopType.Technology => item is Turret ? _technologyTurret : _technologyUpgrade,
			ShopType.Arcane => item is Turret ? _arcaneTurret : _arcaneUpgrade,
			ShopType.Chemical => item is Turret ? _chemicalTurret : _chemicalUpgrade,
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
