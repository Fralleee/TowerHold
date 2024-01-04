using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Enemy : Target
{
	public static Action<Enemy> OnAnyDeath = delegate { };
	public int Value = 10;
	[SerializeField] GameObject _deathEffect;
	public static List<Enemy> AllEnemies = new List<Enemy>();

	[ReadOnly] public int Attackers = 0;
	public bool HasAttackers => Attackers > 0;

	protected override void Awake()
	{
		base.Awake();

		AllEnemies.Add(this);

		Value += GameController.Instance.CurrentLevel * 2;

		OnDeath += HandleDeath;
	}

	void HandleDeath(Target target)
	{
		_ = AllEnemies.Remove(this);
		ResourceManager.Instance.AddResource(Value);

		foreach (var component in GetComponents<MonoBehaviour>())
		{
			component.enabled = false;
		}

		var instance = Instantiate(_deathEffect, Center.position, Quaternion.LookRotation(-transform.forward));
		Destroy(instance, 10f);

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
