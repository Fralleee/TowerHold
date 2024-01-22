using UnityEngine;

[CreateAssetMenu(menuName = "Environment/ObjectSpawner/ClusterSpawnProfile", fileName = "ClusterSpawnProfile")]
public class ClusterSpawnProfile : ObjectSpawnProfile
{
	public int MinClusterSize = 5;
	public int MaxClusterSize = 15;

}
