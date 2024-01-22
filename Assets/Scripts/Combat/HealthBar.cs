using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	[SerializeField] Image _healthBar;
	[SerializeField] Image _transitionHealthbar;
	[SerializeField] GameObject _healthUI;
	[SerializeField] float _transitionDuration = 0.2f;
	[SerializeField] float _delayDuration = 0.5f;
	[SerializeField] float _scaleUpFactor = 1.25f;
	[SerializeField] float _scaleUpDuration = 0.1f;
	[SerializeField] bool _isHidden = true;

	Camera _camera;
	float _maxHealth;
	float _targetValue;
	float _timeElapsed;
	float _startValue;
	bool _doTransition;
	Coroutine _transitionCoroutine;
	Vector3 _originalScale;
	Vector3 _targetScale;
	float _scaleTimeElapsed;
	bool _doScale;

	void Awake()
	{
		_camera = Camera.main;
		_originalScale = _healthUI.transform.localScale;
		_targetScale = new Vector3(_originalScale.x, _originalScale.y * _scaleUpFactor, _originalScale.z);

		var target = GetComponentInParent<Target>();
		_maxHealth = target.MaxHealth;

		if (target is Tower)
		{
			_healthBar.color = Color.green;
		}
		else
		{
			_healthBar.color = Color.red;
		}

		_healthBar.fillAmount = target.Health / _maxHealth;
		_transitionHealthbar.fillAmount = target.Health / _maxHealth;
		_healthUI.transform.localPosition = new Vector3(0f, 3f, 0f);
		_healthUI.SetActive(!_isHidden);
	}

	public void SetMaxHealth(float newMaxHealth) => _maxHealth = newMaxHealth;
	public void SetHealth(float newHP, bool silent = false)
	{
		if (_isHidden)
		{
			_healthUI.SetActive(true);
			_isHidden = false;
		}

		var oldFillAmount = _healthBar.fillAmount;
		_healthBar.fillAmount = newHP / _maxHealth;

		if (silent)
		{
			_targetValue = newHP / _maxHealth;
			return;
		}

		_targetValue = newHP / _maxHealth;
		_startValue = _transitionHealthbar.fillAmount;

		var difference = Mathf.Abs(oldFillAmount - _targetValue);
		if (difference > 0.1f)
		{
			_healthUI.transform.localScale = _targetScale;
			_scaleTimeElapsed = 0;
			_doScale = true;
		}

		if (_transitionCoroutine != null)
		{
			_doTransition = false;
			StopCoroutine(_transitionCoroutine);
		}

		_transitionCoroutine = StartCoroutine(StartDelayedTransition());
	}

	IEnumerator StartDelayedTransition()
	{
		yield return new WaitForSeconds(_delayDuration);

		_timeElapsed = 0;
		_doTransition = true;
	}

	void Update()
	{
		if (_doTransition)
		{
			if (_timeElapsed < _transitionDuration)
			{
				var transitionProgress = _timeElapsed / _transitionDuration;
				_transitionHealthbar.fillAmount = Mathf.Lerp(_startValue, _targetValue, EasingFunctions.OutBack(transitionProgress));
				_timeElapsed += Time.deltaTime;
			}
			else
			{
				_transitionHealthbar.fillAmount = _targetValue;
				_doTransition = false;
			}
		}

		if (_doScale)
		{
			if (_scaleTimeElapsed < _scaleUpDuration)
			{
				var scaleProgress = _scaleTimeElapsed / _scaleUpDuration;
				_healthUI.transform.localScale = Vector3.Lerp(_targetScale, _originalScale, EasingFunctions.OutBack(scaleProgress));
				_scaleTimeElapsed += Time.deltaTime;
			}
			else
			{
				_healthUI.transform.localScale = _originalScale;
				_doScale = false;
			}
		}
	}

	void LateUpdate()
	{
		if (_isHidden)
		{
			return;
		}

		if (_camera)
		{
			transform.LookAt(transform.position + (_camera.transform.rotation * Vector3.forward), _camera.transform.rotation * Vector3.up);
		}
	}
}
