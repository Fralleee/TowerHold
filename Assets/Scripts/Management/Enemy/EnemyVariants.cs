using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyVariants", menuName = "VAKT/Enemy/Spawner/EnemyVariants")]
public class EnemyVariants : ScriptableObject
{
	[AssetList(CustomFilterMethod = "HasEnemyComponent")]
	public Enemy[] Enemies;

#pragma warning disable IDE0051 // Remove unused private members
	bool HasEnemyComponent(GameObject obj)
#pragma warning restore IDE0051 // Remove unused private members
	{
		return obj.GetComponent<Enemy>() != null;
	}
}
