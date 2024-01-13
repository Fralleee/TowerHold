using UnityEngine;

[CreateAssetMenu(menuName = "Environment/ObjectSpawner/Profile", fileName = "ObjectSpawnProfile")]
public class ObjectSpawnProfile : ScriptableObject
{
	public ObjectSpawnerCollection Collection;
	public int StartRadius = 30;
	public int EndRadius = 40;
	public float MinSpacing = 1f;
	public float NoiseScale = 1f;
	public int Intensity = 10;
	public bool CarveNavMesh;
}
