using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Enemy : Target
{
	public static Action<Enemy> OnAnyDeath = delegate { };
	[SerializeField] int _bounty = 10;
	[SerializeField] GameObject _deathEffect;
	[SerializeField] GameObject _model;
	public static List<Enemy> AllEnemies = new List<Enemy>();

	[ReadOnly] public int Attackers = 0;
	public bool HasAttackers => Attackers > 0;
	Animator _animator;

	protected override void Awake()
	{
		base.Awake();

		_animator = GetComponentInChildren<Animator>();

		AllEnemies.Add(this);

		_bounty += GameController.Instance.CurrentLevel * 2;

		OnDeath += HandleDeath;
	}

	void HandleDeath(Target target)
	{
		_ = AllEnemies.Remove(this);
		ResourceManager.Instance.AddResource(_bounty);

		_animator.enabled = false;
		foreach (var component in GetComponents<MonoBehaviour>())
		{
			component.enabled = false;
		}

		var instance = Instantiate(_deathEffect, Center.position, Quaternion.LookRotation(-transform.forward));
		Destroy(instance, 3f);

		Destroy(gameObject);
	}

	public static void ResetGameState()
	{
		AllEnemies.Clear();
		OnAnyDeath = delegate
		{ };
	}

	public static void GameOver()
	{
		foreach (var enemy in AllEnemies)
		{
			enemy._animator.SetTrigger("Victory");
			enemy.GetComponent<MoveToAttack>().Stop();
			foreach (var component in enemy.GetComponents<MonoBehaviour>())
			{
				component.enabled = false;
			}
		}
	}

	public override void Die()
	{
		base.Die();

		OnAnyDeath(this);
	}
}
