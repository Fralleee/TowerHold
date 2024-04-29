using System.Collections.Generic;

public class DamageMultipliers
{
	public float GlobalDamageMultiplier = 1f;
	public Dictionary<ShopType, float> ShopTypeMultipliers = new Dictionary<ShopType, float>() {
		{ ShopType.Force, 1f },
		{ ShopType.Precision, 1f },
		{ ShopType.Technology, 1f },
		{ ShopType.Arcane, 1f },
		{ ShopType.Chemical, 1f }
	};
	public Dictionary<DamageType, float> DamageTypeMultipliers = new Dictionary<DamageType, float>() {
		{ DamageType.Physical, 1f },
		{ DamageType.Magical, 1f },
		{ DamageType.Global, 1f },
	};

	public void AddUppgrade(DamageUpgrade damageUpgrade)
	{
		if (damageUpgrade.ShopType == ShopType.Offense)
		{
			if (damageUpgrade.DamageType == DamageType.Global)
			{
				GlobalDamageMultiplier += damageUpgrade.Amount;
			}
			else
			{
				DamageTypeMultipliers[damageUpgrade.DamageType] += damageUpgrade.Amount;
			}
		}
		else
		{
			ShopTypeMultipliers[damageUpgrade.ShopType] += damageUpgrade.Amount;
		}
	}
	public float GetMultiplier(DamageType damageType, ShopType shopType)
	{
		return DamageTypeMultipliers[damageType] * ShopTypeMultipliers[shopType] * GlobalDamageMultiplier;
	}
}
