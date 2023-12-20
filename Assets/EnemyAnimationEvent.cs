using UnityEngine;

public class EnemyAnimationEvent : MonoBehaviour
{
	EnemyAttack _enemyAttack;

	void Awake() => _enemyAttack = GetComponentInParent<EnemyAttack>();

	void PerformAttack()
	{
		_enemyAttack.PerformAttack();
	}
}
