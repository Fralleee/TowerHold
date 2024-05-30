using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpawnDecals : MonoBehaviour
{
	[SerializeField] DecalProjector _decalPrefab;
	[SerializeField] Material[] _decalMaterials;

	void Start()
	{
		var ray = new Ray(transform.position + (Vector3.up * 10), Vector3.down);
		if (Physics.Raycast(ray, out var hit, 100, LayerMask.GetMask("Ground")))
		{
			var rotation = Quaternion.Euler(90, Random.Range(0, 360), 0);
			var decal = Instantiate(_decalPrefab, hit.point, rotation);
			decal.material = _decalMaterials[Random.Range(0, _decalMaterials.Length)];
		}

	}
}
