using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class SelectionController : Controller
{
	[SerializeField] LayerMask _selectableLayer;
	[SerializeField] GameObject _selectionIndicator;

	Camera _mainCamera;
	Target _selectedTarget;
	Label _nameLabel;
	VisualElement _container;
	CustomProgressBar _healthBar;

	EventBinding<TargetDeathEvent> _targetDeathEvent;
	EventBinding<TargetHealthChangedEvent> _targetHealthChangedEvent;

	void OnEnable()
	{
		_targetDeathEvent = new EventBinding<TargetDeathEvent>(HandleTargetDeath);
		EventBus<TargetDeathEvent>.Register(_targetDeathEvent);

		_targetHealthChangedEvent = new EventBinding<TargetHealthChangedEvent>(HandleTargetHealthChanged);
		EventBus<TargetHealthChangedEvent>.Register(_targetHealthChangedEvent);
	}

	void HandleTargetHealthChanged(TargetHealthChangedEvent e)
	{
		if (e.Target == _selectedTarget)
		{
			OnHealthChanged(e.Health, e.MaxHealth);
		}
	}

	void HandleTargetDeath(TargetDeathEvent @event)
	{
		if (@event.Target == _selectedTarget)
		{
			DeselectUnit(_selectedTarget);
		}
	}

	void OnDisable()
	{
		EventBus<TargetDeathEvent>.Deregister(_targetDeathEvent);
		EventBus<TargetHealthChangedEvent>.Deregister(_targetHealthChangedEvent);
	}

	protected override void Awake()
	{
		base.Awake();

		var uiDocument = GetComponent<UIDocument>();
		var rootElement = uiDocument.rootVisualElement;
		rootElement.SetPickingModeRecursive(PickingMode.Ignore);

		_container = rootElement.Q<VisualElement>("SelectionContainer");
		_container.AddToClassList("no-target");

		_healthBar = _container.Q<CustomProgressBar>("SelectionHealthBar");
		_healthBar.UseChangeBar = false;

		_nameLabel = _container.Q<Label>("SelectionName");

		_mainCamera = Camera.main;
	}

	void Start()
	{
		if (_selectionIndicator != null)
		{
			_selectionIndicator = Instantiate(_selectionIndicator);
			_selectionIndicator.SetActive(false);
		}
	}

	void Update()
	{
		if (Controls.Mouse.PrimaryFireTap.WasPerformedThisFrame())
		{
			OnPrimaryFire();
		}
	}

	void OnPrimaryFire()
	{
		if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}

		DeselectUnit(_selectedTarget);
		var ray = _mainCamera.ScreenPointToRay(MousePosition);
		if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _selectableLayer))
		{
			var hitObject = hit.transform.gameObject;
			var target = hitObject.GetComponent<Target>();
			if (target)
			{
				SelectUnit(target);
			}
		}
	}

	void OnHealthChanged(int currentHealth, int maxHealth)
	{
		_healthBar.MinMaxValue = (currentHealth, maxHealth);
		_healthBar.Value = currentHealth / (float)maxHealth;
	}

	void SelectUnit(Target target)
	{
		_selectedTarget = target;
		_nameLabel.text = _selectedTarget.name;
		_container.RemoveFromClassList("no-target");

		if (_selectionIndicator != null)
		{
			_selectionIndicator.transform.SetParent(_selectedTarget.transform);
			_selectionIndicator.transform.localPosition = Vector3.up;
			_selectionIndicator.SetActive(true);

			_selectionIndicator.transform.localScale = Vector3.one * _selectedTarget.Scale;
		}
	}

	void DeselectUnit(Target _)
	{
		if (_selectedTarget != null)
		{
			_selectedTarget = null;

			_container.AddToClassList("no-target");

			if (_selectionIndicator != null)
			{
				_selectionIndicator.transform.parent = null;
				_selectionIndicator.SetActive(false);
			}
		}
	}
}
