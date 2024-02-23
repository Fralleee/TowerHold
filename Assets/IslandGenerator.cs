using Sirenix.OdinInspector;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    [SerializeField] GameObject _baseMeshPrefab; // Assign in the Inspector
    [SerializeField] GameObject[] _detailMeshPrefabs; // Assign multiple detail prefabs in the Inspector
    [SerializeField] int _numDetails = 4; // Number of details to place around the base mesh
    [SerializeField] float _radius = 128f; // Radius of the circular base mesh


    [Button]
    void Generate()
    {
        ClearChildren(transform); // Clear any existing children before generating new ones
        // Instantiate the base mesh at the center and set it as a child of this GameObject
        var baseMesh = Instantiate(_baseMeshPrefab, transform.position, Quaternion.identity, transform);

        // Calculate the angle step based on the number of details
        var angleStep = 360f / _numDetails;

        for (var i = 0; i < _numDetails; i++)
        {
            // Calculate the position for each detail
            var angle = i * angleStep;
            var detailPosition = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad)) * _radius;
            detailPosition += transform.position; // Adjust based on the parent's position

            // Randomly select a detail mesh prefab and instantiate it
            var detailPrefab = _detailMeshPrefabs[Random.Range(0, _detailMeshPrefabs.Length)];


            var detailMesh = Instantiate(detailPrefab, detailPosition, Quaternion.identity, transform);

            // Rotate the detail mesh to face outwards from the center
            var direction = (detailPosition - transform.position).normalized;
            detailMesh.transform.rotation = Quaternion.LookRotation(direction);

            // Optionally, adjust the detail mesh's position/rotation further if needed
        }
    }

    void ClearChildren(Transform t)
    {
        if (t.childCount == 0)
        {
            return;
        }
        else
        {
            for (var i = t.childCount - 1; i >= 0; i--)
            {
                ClearChildren(t.GetChild(i));
                DestroyImmediate(t.GetChild(i).gameObject);
            }
        }
    }
}
