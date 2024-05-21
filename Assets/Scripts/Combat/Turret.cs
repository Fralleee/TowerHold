using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Shop/Turret")]
public class Turret : DamageShopItem
{
	[EnumToggleButtons] public AttackRange AttackRange = AttackRange.Short;

	[Header("Settings")]
	[MinValue(1f)] public float BaseDamage = 10f;
	[Unit(Units.Second), MinValue(0.5f)] public float TimeBetweenAttacks = 1f;
	[ReadOnly] public float DPS;
	[SerializeField] AudioClip _attackSound;

	[Header("Critical hit")]
	[Range(0, 1)] public float CriticalHitChance = 0f;
	[HideIf("@CriticalHitChance==0")] public float CriticalHitMultiplier = 2f;

	[Header("Behaviors")]
	[InlineEditor(InlineEditorModes.GUIOnly)] public AttackType AttackType;
	public AfflictionsController AfflictionsController;
	public bool PreferNewTarget = false;
	public override bool IsTurretType => true;

	float _lastAttackTime = 0f;
	Tower _tower;
	AudioSource _audioSource;

	public void Setup(Tower inputTower)
	{
		_tower = inputTower;
		_lastAttackTime = GameController.Instance.RandomGenerator.NextFloat(0f, TimeBetweenAttacks); // Add random delay for the first attack
		_audioSource = _tower.GetComponent<AudioSource>();
		PreferNewTarget = AfflictionsController.PreferNewTarget;
	}

	public void FixedUpdate()
	{
		if (Time.time - _lastAttackTime > TimeBetweenAttacks)
		{
			Attack();
		}
	}

	public void Attack(bool executeBehaviors = true, Affliction excludeBehavior = null)
	{
		var targets = AttackType.AcquireTargets(this);
		if (targets.Count() > 0)
		{
			AttackType.ExecuteAttack(targets, this, executeBehaviors, excludeBehavior);
		}

		_lastAttackTime = Time.time;
	}

	public override void OnPurchase()
	{
		base.OnPurchase();

		Tower.Instance.AddTurret(this);
		ScoreManager.Instance.Turrets += 1;
	}

	public void PlayAttackSound()
	{
		if (_attackSound != null && _audioSource != null)
		{
			_audioSource.PlayOneShot(_attackSound);
		}
		else
		{
			Debug.LogWarning("EnemyAttack: No audio source or attack sound assigned to enemy.");
		}
	}

	public override TooltipContent Tooltip(StyleSettings styleSettings)
	{
		var tooltip = new TurretTooltipContent();
		tooltip.UpdateInformation(this, styleSettings);
		return tooltip;
	}

	void OnValidate()
	{
		Description = $"Shoots towards nearest enemy causing #Damage# damage.";
		DPS = BaseDamage / TimeBetweenAttacks;
	}
}
