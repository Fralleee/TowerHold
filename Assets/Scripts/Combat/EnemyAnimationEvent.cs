using UnityEngine;

public class EnemyAnimationEvent : MonoBehaviour
{
	Enemy _enemy;

	void Awake() => _enemy = GetComponentInParent<Enemy>();

	public void PerformAttack()
	{
		_enemy.PerformAttack();
	}
}
