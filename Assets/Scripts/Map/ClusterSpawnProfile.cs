using UnityEngine;

[CreateAssetMenu(fileName = "ClusterSpawnProfile", menuName = "VAKT/Environment/ObjectSpawner/ClusterSpawnProfile")]
public class ClusterSpawnProfile : ObjectSpawnProfile
{
	public int MinClusterSize = 5;
	public int MaxClusterSize = 15;

}
