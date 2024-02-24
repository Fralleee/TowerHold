using UnityEngine;

[CreateAssetMenu(fileName = "ObjectSpawnerCollection", menuName = "VAKT/Environment/ObjectSpawner/Collection")]
public class ObjectSpawnerCollection : ScriptableObject
{
	public SpawnableObject[] Objects;
}
