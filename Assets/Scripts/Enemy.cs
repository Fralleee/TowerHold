using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Enemy : Target
{
	public static Action<Enemy> OnAnyDeath = delegate { };
	[SerializeField] int _bounty = 10;
	public static List<Enemy> AllEnemies = new List<Enemy>();

	[ReadOnly] public int Attackers = 0;
	public bool HasAttackers => Attackers > 0;
	Animator _animator;
	Rigidbody[] _rigidbodies;

	protected override void Awake()
	{
		base.Awake();

		_animator = GetComponentInChildren<Animator>();

		AllEnemies.Add(this);

		// Increase bounty based on level
		_bounty += GameController.Instance.CurrentLevel * 2;

		OnDeath += HandleDeath;

		_rigidbodies = GetComponentsInChildren<Rigidbody>().ToArray();
	}

	void HandleDeath(Target target)
	{
		_ = AllEnemies.Remove(this);
		ResourceManager.Instance.AddResource(_bounty);

		// Disable all monobehaviours on gameObject
		_animator.enabled = false;
		foreach (var component in GetComponents<MonoBehaviour>())
		{
			component.enabled = false;
		}

		var offset = transform.forward + transform.up;
		foreach (var rigidBody in _rigidbodies)
		{
			rigidBody.isKinematic = false;
			rigidBody.velocity = Vector3.zero;
			rigidBody.AddForceAtPosition(-transform.forward * 20f, offset, ForceMode.Impulse);
		}

		Destroy(gameObject, 3f);
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
