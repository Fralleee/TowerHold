using UnityEngine;

public class Clouds : MonoBehaviour
{
	[SerializeField] float _rotationSpeed = 10f;

	void Update()
	{
		transform.RotateAround(transform.position, Vector3.up, _rotationSpeed * Time.deltaTime);
	}
}
