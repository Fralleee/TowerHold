using UnityEngine;

[CreateAssetMenu(menuName = "Environment/ObjectSpawner/Collection", fileName = "ObjectSpawnerCollection")]
public class ObjectSpawnerCollection : ScriptableObject
{
	public SpawnableObject[] Objects;
}
