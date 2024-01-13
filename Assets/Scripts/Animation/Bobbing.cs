using UnityEngine;

public class Bobbing : MonoBehaviour
{
	[SerializeField]
	float _bobbingAmount = 0.125f;
	[SerializeField]
	float _bobbingSpeed = 25f;
	[SerializeField]
	float _rotationAmount = 5f;
	[SerializeField]
	float _rotationSpeed = 10f;  // New field for rotation speed

	Vector3 _startingPosition;
	Quaternion _startingRotation;
	float _bobbingTimer;
	float _rotationTimer;  // Separate timer for rotation
	bool _isStopping;
	bool _isStopped;
	float _stopProgress;

	void Start()
	{
		_startingPosition = transform.localPosition;
		_startingRotation = transform.localRotation;

		var randomTimer = RandomManager.Delay(0f, 2f * Mathf.PI);
		_bobbingTimer = randomTimer;
		_rotationTimer = randomTimer;
	}

	void Update()
	{
		if (_isStopped)
		{
			return;
		}

		if (_isStopping)
		{
			_stopProgress += Time.deltaTime * _bobbingSpeed;

			transform.SetLocalPositionAndRotation(Vector3.Lerp(transform.localPosition, _startingPosition, _stopProgress), Quaternion.Lerp(transform.localRotation, _startingRotation, _stopProgress));

			if (_stopProgress >= 1f)
			{
				_isStopping = false;
				_isStopped = true;
			}
		}
		else
		{

			_bobbingTimer += Time.deltaTime * _bobbingSpeed;
			_rotationTimer += Time.deltaTime * _rotationSpeed;  // Increment rotation timer

			var bobbingOffset = Mathf.Sin(_bobbingTimer) * _bobbingAmount;
			var rotationOffset = Mathf.Sin(_rotationTimer) * _rotationAmount;  // Use rotation timer
			transform.SetLocalPositionAndRotation(_startingPosition + new Vector3(0f, bobbingOffset + _bobbingAmount, 0f), _startingRotation * Quaternion.Euler(0f, rotationOffset, rotationOffset));
		}
	}

	public void Stop()
	{
		_isStopping = true;
	}
}
