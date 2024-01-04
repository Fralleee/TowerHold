using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Spawner/EnemyVariants", fileName = "EnemyVariants")]
public class EnemyVariants : ScriptableObject
{
	public Enemy[] Enemies;
}
