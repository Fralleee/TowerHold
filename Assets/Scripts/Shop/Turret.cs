using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "VAKT/Shop/Turret")]
public class Turret : DamageShopItem
{
	[Header("Settings")]
	public float BaseDamage = 10f;
	public AttackRange AttackRange = AttackRange.Short;
	public float TimeBetweenAttacks = 1f;
	public float CriticalHitChance = 0f;
	public float CriticalHitMultiplier = 2f;

	[Header("Audio")]
	[SerializeField] AudioClip _attackSound;
	[SerializeField] AudioSettings _audioSettings;

	[Header("Behaviors")]
	[InlineEditor(InlineEditorModes.GUIOnly)] public AttackType AttackType;
	public Afflictions Afflictions;
	public bool PreferNewTarget = false;

	float _lastAttackTime = 0f;
	Tower _tower;
	AudioSource _audioSource;

	public void Reset()
	{
		Description = "Shoots towards nearest enemy causing #Damage# damage.";
	}

	public void Setup(Tower inputTower)
	{
		_tower = inputTower;
		_lastAttackTime = GameController.Instance.RandomGenerator.NextFloat(0f, TimeBetweenAttacks); // Add random delay for the first attack
		_audioSource = _tower.GetComponent<AudioSource>();
		Afflictions = new Afflictions(this);
		PreferNewTarget = Afflictions.PreferNewTarget;
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
			_audioSettings.ApplySettings(_audioSource);
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
}
