using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public static Health asTarget;
    public Transform origin;

    public List<Turret> startTurrets;
    public List<Turret> turrets;

    public List<Upgrade> startUpgrades;
    public Dictionary<DamageType, float> damageMultipliers = new Dictionary<DamageType, float>() {
        { DamageType.Normal, 1f },
        { DamageType.Piercing, 1f },
        { DamageType.Siege, 1f },
        { DamageType.Magic, 1f },
        { DamageType.Poison, 1f },
        { DamageType.Chaos, 1f }
    };

    void Awake()
    {
        asTarget = GetComponent<Health>();


    }

    void Start()
    {
        foreach (var turret in startTurrets)
        {
            AddTurret(turret);
        }

        foreach (var upgrade in startUpgrades)
        {
            AddUppgrade(upgrade.damageType);
        }
    }

    void Update()
    {
        foreach (var turret in turrets)
        {
            turret.Update();
        }
    }

    public void AddTurret(Turret turret)
    {
        var instance = Instantiate(turret);
        instance.Setup(this);
        turrets.Add(instance);
    }

    public void AddUppgrade(DamageType damageType)
    {
        damageMultipliers[damageType] += 0.1f;
    }

    public float GetDamage(DamageType damageType, float damage)
    {
        Debug.Log("Getting damage: " + damage + " with type: " + damageType);
        Debug.Log("Damage multiplier: " + damageMultipliers[damageType]);
        Debug.Log("Final damage: " + damage * damageMultipliers[damageType]);
        return damage * damageMultipliers[damageType];
    }
}
