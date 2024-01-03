using UnityEngine;

[CreateAssetMenu(menuName = "Environment/ObjectSpawner/Settings", fileName = "ObjectSpawnerSettings")]
public class ObjectSpawnerSettings : ScriptableObject
{
	public ObjectSpawnerCollection Collection;
	public int StartRadius = 30;
	public int EndRadius = 40;
	public LayerMask ObstructionLayer;
	public float MinSpacing = 1f;
	public int Intensity = 10;
	public bool CarveNavMesh;
}
