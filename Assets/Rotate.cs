using UnityEngine;

public class Rotate : MonoBehaviour
{
	[SerializeField] Space _rotationSpace = Space.Self;
	[SerializeField] Vector3 _rotationSpeed = new Vector3(0, 90f, 0f);

	void Update()
	{
		transform.Rotate(_rotationSpeed * Time.deltaTime, _rotationSpace);
	}
}
