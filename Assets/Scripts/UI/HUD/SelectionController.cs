using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SelectionController : Controller
{
	[SerializeField] LayerMask _selectableLayer;
	[SerializeField] GameObject _selectionIndicator;

	Camera _mainCamera;
	Target _selectedTarget;
	Label _nameLabel;

	protected override void Awake()
	{
		base.Awake();

		var uiDocument = GetComponent<UIDocument>();
		var rootElement = uiDocument.rootVisualElement;
		_nameLabel = rootElement.Q<Label>("SelectedTarget");
		_nameLabel.AddToClassList("no-target");

		_mainCamera = Camera.main;

		Controls.Mouse.PrimaryFireTap.performed += OnPrimaryFire;
	}

	void Start()
	{
		if (_selectionIndicator != null)
		{
			_selectionIndicator = Instantiate(_selectionIndicator);
			_selectionIndicator.SetActive(false);
		}
	}

	void OnDisable()
	{
		Controls.Mouse.PrimaryFireTap.performed -= OnPrimaryFire;
	}

	void OnPrimaryFire(InputAction.CallbackContext context)
	{
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

	void SelectUnit(Target target)
	{
		_selectedTarget = target;

		_nameLabel.text = _selectedTarget.name;
		_nameLabel.RemoveFromClassList("no-target");

		if (_selectionIndicator != null)
		{
			_selectionIndicator.transform.SetParent(_selectedTarget.transform);
			_selectionIndicator.transform.localPosition = Vector3.up;
			_selectionIndicator.SetActive(true);

			_selectionIndicator.transform.localScale = Vector3.one * _selectedTarget.Scale;
		}

		_selectedTarget.OnDeath += DeselectUnit;
	}

	void DeselectUnit(Target _)
	{
		if (_selectedTarget != null)
		{
			_selectedTarget.OnDeath -= DeselectUnit;
			_selectedTarget = null;

			_nameLabel.AddToClassList("no-target");

			if (_selectionIndicator != null)
			{
				_selectionIndicator.transform.parent = null;
				_selectionIndicator.SetActive(false);
			}
		}
	}
}
