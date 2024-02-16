using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
	TextMeshPro _textMeshPro;

	public float ScaleUpTime = 0.5f;
	public float FadeOutTime = 1f;
	public float Lifetime = 2f;
	public float AnimateDistance = 4f;

	public AnimationCurve FadeInCurve;
	public AnimationCurve FadeOutCurve;

	static int _sortingOrderCounter = 0;

	float _elapsedTime;
	float _scaleMultiplier;

	Vector3 _initialPosition;
	Vector3 _targetPosition;
	Vector3 _baseScale = Vector3.one;

	Camera _camera;

	public FloatingText Spawn(Vector3 newPosition, string value)
	{
		var rotation = Quaternion.Euler(0, 0, Random.Range(-15f, 15f));
		var floatingText = Instantiate(this, newPosition, rotation);

		floatingText.Setup(value);

		return floatingText;
	}

	public void Setup(string value)
	{
		_textMeshPro = GetComponent<TextMeshPro>();
		_textMeshPro.text = value;

		// Set the sorting order based on the static counter and then increment the counter
		_textMeshPro.sortingOrder = _sortingOrderCounter++;
	}

	void Start()
	{
		_camera = Camera.main;
		_initialPosition = transform.localPosition;
		_targetPosition = _initialPosition + (transform.up * AnimateDistance);
	}

	void Update()
	{
		_elapsedTime += Time.deltaTime;
		if (_elapsedTime > Lifetime)
		{
			Remove();
		}

		Animate();
	}

	void LateUpdate()
	{
		AdjustScale();
		FaceCamera();
	}

	void Animate()
	{
		var time = _elapsedTime / Lifetime;
		transform.localPosition = Vector3.Lerp(_initialPosition, _targetPosition, FadeInCurve.Evaluate(time));
		if (_elapsedTime < ScaleUpTime)
		{
			_scaleMultiplier = Mathf.Lerp(0, 1, _elapsedTime / ScaleUpTime);
		}
		else
		{
			_scaleMultiplier = 1;
		}

		if (_elapsedTime > Lifetime - FadeOutTime)
		{
			_textMeshPro.alpha = FadeOutCurve.Evaluate(time);
		}
	}

	// Scale based on camera distance to maintain consistent size
	void AdjustScale()
	{
		var distance = Vector3.Distance(transform.position, _camera.transform.position);
		transform.localScale = _scaleMultiplier * (1 + (distance * 0.1f)) * _baseScale; // Apply scaling on base scale
	}

	void FaceCamera()
	{
		var toCamera = _camera.transform.position - transform.position;
		var forward = -toCamera; // Make the text face away from the camera by inverting the direction

		// Keep the text upright by using the global up vector
		forward.y = 0; // Remove any vertical component to keep the text upright

		var targetRotation = Quaternion.LookRotation(forward, Vector3.up);

		// Preserve the original Z rotation
		targetRotation.eulerAngles = new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.eulerAngles.z);

		transform.rotation = targetRotation;
	}

	void Remove()
	{
		Destroy(gameObject);
	}
}
