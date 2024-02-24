using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class FloatingText : MonoBehaviour
{
	TextMeshPro _textMeshPro;

	public float ScaleUpTime = 0.07f;
	public float FadeOutTime = 0.5f;
	public float Lifetime = 1.5f;
	public float AnimateDistance = 4f;
	public bool UseConsistentScale;

	public AnimationCurve FadeInCurve;

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
		_textMeshPro.fontSize *= Random.Range(0.9f, 1.1f);
		_textMeshPro.sortingOrder = _sortingOrderCounter++;
	}

	void Start()
	{
		_camera = Camera.main;
		_initialPosition = transform.localPosition;

		var randomDirection = new Vector3(Random.Range(-0.75f, 0.75f), 1, Random.Range(-0.75f, 0.75f));
		_targetPosition = _initialPosition + (randomDirection * AnimateDistance);
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
		if (UseConsistentScale)
		{
			AdjustScaleConsistent();
		}
		else
		{
			AdjustScale();
		}

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
		else if (_elapsedTime > (Lifetime - FadeOutTime))
		{
			time = _elapsedTime - (Lifetime - FadeOutTime);
			_scaleMultiplier = Mathf.Lerp(1, 0, time / FadeOutTime);
		}
		else
		{
			_scaleMultiplier = 1;
		}
	}

	// Scale based on camera distance to maintain consistent size
	void AdjustScaleConsistent()
	{
		var distance = Vector3.Distance(transform.position, _camera.transform.position);
		var scale = _scaleMultiplier * (1 + (distance * 0.1f)) * _baseScale;

		transform.localScale = scale;
	}

	// Scale based on camera distance
	// The text will appear larger when closer to the camera
	// The text will appear smaller when further from the camera
	void AdjustScale()
	{
		var distance = Vector3.Distance(transform.position, _camera.transform.position);
		var scale = _scaleMultiplier * (1 + (distance * 0.1f)) * _baseScale;

		var minDistance = 60f;
		var maxDistance = 420f;
		var scaleMultiplier = Mathf.InverseLerp(minDistance, maxDistance, distance);
		scale *= Mathf.Lerp(1.3f, 0.7f, scaleMultiplier);

		transform.localScale = scale;
	}

	void FaceCamera()
	{
		var toCamera = _camera.transform.position - transform.position;
		var forward = -toCamera; // Make the text face away from the camera by inverting the direction
		forward.y = 0; // Remove any vertical component to keep the text upright

		var targetRotation = Quaternion.LookRotation(forward, Vector3.up);
		targetRotation.eulerAngles = new Vector3(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.eulerAngles.z);
		transform.rotation = targetRotation;
	}

	void Remove()
	{
		Destroy(gameObject);
	}
}
