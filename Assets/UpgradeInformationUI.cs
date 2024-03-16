using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeInformationUI : MonoBehaviour
{
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

	void Awake()
	{
		var uiDocument = GetComponent<UIDocument>();
		var attackTypesList = uiDocument.rootVisualElement.Q<Box>("AttackTypesList");

		_incomeUpgrade = new UpgradeInformation(attackTypesList.Q<Label>("income-upgrade-label"));
		_defenseUpgrade = new UpgradeInformation(attackTypesList.Q<Label>("defense-upgrade-label"));
		_normalUpgrade = new UpgradeInformation(attackTypesList.Q<Label>("normal-upgrade-label"));
		_normalTurret = new UpgradeInformation(attackTypesList.Q<Label>("normal-turret-label"));
		_siegeUpgrade = new UpgradeInformation(attackTypesList.Q<Label>("siege-upgrade-label"));
		_siegeTurret = new UpgradeInformation(attackTypesList.Q<Label>("siege-turret-label"));
		_technologyUpgrade = new UpgradeInformation(attackTypesList.Q<Label>("technology-upgrade-label"));
		_technologyTurret = new UpgradeInformation(attackTypesList.Q<Label>("technology-turret-label"));
		_arcaneUpgrade = new UpgradeInformation(attackTypesList.Q<Label>("arcane-upgrade-label"));
		_arcaneTurret = new UpgradeInformation(attackTypesList.Q<Label>("arcane-turret-label"));
		_voidUpgrade = new UpgradeInformation(attackTypesList.Q<Label>("void-upgrade-label"));
		_voidTurret = new UpgradeInformation(attackTypesList.Q<Label>("void-turret-label"));

		Tower.OnUpgrade += OnUpgrade;
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
		Tower.OnUpgrade -= OnUpgrade;
	}
}
