using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
  [SerializeField] Image _healthBar;
  [SerializeField] GameObject _healthUI;
  [SerializeField] float _transitionDuration = 0.2f;
  [SerializeField] bool _isHidden = true;

  Camera _camera;
  float _maxHealth;
  float _targetValue;
  float _timeElapsed;
  float _startValue;
  bool _doTransition;

  void Awake()
  {
    _camera = Camera.main;

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
    _healthUI.transform.localPosition = new Vector3(0f, 3f, 0f);
    _healthUI.SetActive(!_isHidden);
  }

  public void SetHealth(float newHP)
  {
    if (_isHidden)
    {
      _healthUI.SetActive(true);
      _isHidden = false;
    }

    _startValue = _healthBar.fillAmount;
    _targetValue = newHP / _maxHealth;
    _timeElapsed = 0;
    _doTransition = true;
  }

  void Update()
  {
    if (_doTransition)
    {
      if (_timeElapsed < _transitionDuration)
      {
        _healthBar.fillAmount = Mathf.Lerp(_startValue, _targetValue, EasingFunctions.OutBack(_timeElapsed / _transitionDuration));
        _timeElapsed += Time.deltaTime;
      }
      else
      {
        _healthBar.fillAmount = _targetValue;
        _doTransition = false;
      }
    }
  }

  void LateUpdate()
  {
    if (_isHidden) return;

    if (_camera) transform.LookAt(transform.position + (_camera.transform.rotation * Vector3.forward), _camera.transform.rotation * Vector3.up);
  }
}