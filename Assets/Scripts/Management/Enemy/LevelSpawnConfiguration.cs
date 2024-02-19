using System;

[Serializable]
public struct LevelSpawnConfiguration
{
	public int Level;
	public bool SpawnAsGroup;
	public bool SpawnOnce;
	public bool ContinueDefaultSpawns;
	public Enemy[] Enemies;
}
