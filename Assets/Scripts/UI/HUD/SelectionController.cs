using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

	protected override void Awake()
	{
		base.Awake();

		var uiDocument = GetComponent<UIDocument>();
		var rootElement = uiDocument.rootVisualElement;
		SetPickingModeRecursive(rootElement);

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
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}

		DeselectUnit(_selectedTarget);
		var ray = _mainCamera.ScreenPointToRay(MousePosition);
		if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _selectableLayer))
		{
			var hitObject = hit.transform.gameObject;
			var target = hitObject.GetComponentInParent<Target>();
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
		_selectedTarget.OnHealthChanged += OnHealthChanged;
		_selectedTarget.OnDeath += DeselectUnit;
		OnHealthChanged(_selectedTarget.Health, _selectedTarget.MaxHealth);

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
			_selectedTarget.OnHealthChanged -= OnHealthChanged;
			_selectedTarget.OnDeath -= DeselectUnit;
			_selectedTarget = null;

			_container.AddToClassList("no-target");

			if (_selectionIndicator != null)
			{
				_selectionIndicator.transform.parent = null;
				_selectionIndicator.SetActive(false);
			}
		}
	}
	void SetPickingModeRecursive(VisualElement element)
	{
		if (element is Button or Label or CustomProgressBar)
		{
			return;
		}

		element.pickingMode = PickingMode.Ignore;
		foreach (var child in element.Children())
		{
			SetPickingModeRecursive(child);
		}
	}
}
