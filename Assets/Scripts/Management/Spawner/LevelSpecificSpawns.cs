using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Spawner/LevelSpecificSpawns", fileName = "LevelSpecificSpawns")]
public class LevelSpecificSpawns : ScriptableObject, IEnumerable<LevelSpawnConfiguration>
{
	public LevelSpawnConfiguration[] List;

	public IEnumerator<LevelSpawnConfiguration> GetEnumerator()
	{
		return ((IEnumerable<LevelSpawnConfiguration>)List).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
