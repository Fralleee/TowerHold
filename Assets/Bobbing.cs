using UnityEngine;

public class Bobbing : MonoBehaviour
{
	[SerializeField]
	float _bobbingAmount = 0.1f;
	[SerializeField]
	float _bobbingSpeed = 20f;
	[SerializeField]
	float _rotationAmount = 5f;

	Vector3 _startingPosition;
	Quaternion _startingRotation;
	float _timer;
	bool _isStopping;
	bool _isStopped;
	float _stopProgress;

	void Start()
	{
		_startingPosition = transform.localPosition;
		_startingRotation = transform.localRotation;
	}

	void Update()
	{
		if (_isStopped)
		{
			return;
		}

		_timer += Time.deltaTime * _bobbingSpeed;

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
			var bobbingOffset = Mathf.Sin(_timer) * _bobbingAmount;
			var rotationOffset = Mathf.Sin(_timer) * _rotationAmount;
			transform.SetLocalPositionAndRotation(_startingPosition + new Vector3(0f, bobbingOffset, 0f), _startingRotation * Quaternion.Euler(0f, 0f, rotationOffset));
		}
	}

	public void Stop()
	{
		_isStopping = true;
	}
}
