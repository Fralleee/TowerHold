using UnityEngine.UIElements;

public class ItemInformation : VisualElement
{
	public new class UxmlFactory : UxmlFactory<ItemInformation, UxmlTraits> { }

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		// Add attributes here if needed, for example, you might want an attribute for the item name, cost, etc.
	}

	Label _nameLabel;
	Label _costLabel;
	VisualElement _divider;
	Label _typeLabel;
	Label _damageTypeLabel;
	Label _damageLabel;
	Label _dpsLabel;
	Label _attackCooldownLabel;
	Label _rangeLabel;

	public ItemInformation()
	{
		AddToClassList("item-information");

		SetupUI();
	}

	void SetupUI()
	{
		_nameLabel = new Label("Name");
		_nameLabel.AddToClassList("name");
		Add(_nameLabel);

		_costLabel = new Label("Cost");
		_costLabel.AddToClassList("cost");
		Add(_costLabel);

		_divider = new VisualElement();
		_divider.AddToClassList("divider");
		Add(_divider);

		_typeLabel = new Label("Type");
		Add(_typeLabel);

		_damageTypeLabel = new Label("DamageType");
		Add(_damageTypeLabel);

		_damageLabel = new Label("Damage");
		Add(_damageLabel);

		_dpsLabel = new Label("DPS");
		Add(_dpsLabel);

		_attackCooldownLabel = new Label("Attack Cooldown");
		Add(_attackCooldownLabel);

		_rangeLabel = new Label("Range");
		Add(_rangeLabel);
	}

	public void UpdateItemInformation(ShopItem item)
	{
		_nameLabel.text = item.name;
		_costLabel.text = item.Cost.ToString();
		_damageTypeLabel.text = $"Type: {item.ShopType}";

		if (item is Turret turret)
		{
			var (baseDamage, attackRange, timeBetweenAttacks) = turret.GetHoverData();
			_typeLabel.text = "Weapon";
			_damageLabel.text = $"Damage: {baseDamage}";
			_dpsLabel.text = $"DPS: {baseDamage / timeBetweenAttacks}";
			_attackCooldownLabel.text = $"Attack Cooldown: {timeBetweenAttacks}";
			_rangeLabel.text = $"Range: {attackRange}";
			_typeLabel.style.display = DisplayStyle.Flex;
			_damageLabel.style.display = DisplayStyle.Flex;
			_dpsLabel.style.display = DisplayStyle.Flex;
			_attackCooldownLabel.style.display = DisplayStyle.Flex;
			_rangeLabel.style.display = DisplayStyle.Flex;
		}
		else
		{
			_typeLabel.style.display = DisplayStyle.None;
			_damageLabel.style.display = DisplayStyle.None;
			_dpsLabel.style.display = DisplayStyle.None;
			_attackCooldownLabel.style.display = DisplayStyle.None;
			_rangeLabel.style.display = DisplayStyle.None;
		}
	}
}
