using System;

[Serializable]
public struct LevelSpawnConfiguration
{
	public int Level;
	public bool SpawnAsGroup;
	public Enemy[] Enemies;
}
