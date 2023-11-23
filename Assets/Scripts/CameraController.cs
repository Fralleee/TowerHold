using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : Controller
{
	[SerializeField] Transform _cameraTransform;
	[SerializeField] float _keyboardMovementSpeed = 0.25f;
	[SerializeField] float _zoomAmount = 10;
	[SerializeField] float _maxZoomAmount = 100;
	[SerializeField] float _smoothingTime = 10;

	Camera _mainCam;
	Transform _transform;

	Vector3 _zoomVector;
	Vector3 _newPosition;
	Vector3 _newZoom;
	Vector3 _dragStartPosition;
	Vector3 _rotateStartPosition;
	Vector3 _cameraLocalPosition;
	Quaternion _newRotation;

	bool _isMouseMoving;
	bool _isMouseRotating;
	bool IsKeyboardMoving => KeyboardMovement.magnitude != 0;
	bool IsKeyboardRotating => KeyboardRotation != 0;

	void Start()
	{
		_transform = transform;
		_mainCam = _cameraTransform.GetComponentInChildren<Camera>();

		Controls.Mouse.PrimaryFire.started += MoveStart;
		Controls.Mouse.PrimaryFire.canceled += MoveCanceled;
		Controls.Mouse.Rotation.started += RotationStart;
		Controls.Mouse.Rotation.canceled += RotationCanceled;
		Controls.Mouse.Scroll.performed += Scroll;

		_zoomVector = new Vector3(0, -_zoomAmount, _zoomAmount);

		_newPosition = _transform.position;
		_newRotation = _transform.rotation;

		_cameraLocalPosition = _cameraTransform.localPosition;
	}

	void LateUpdate()
	{
		if (_isMouseMoving)
		{
			HandleMovement();
		}
		else if (IsKeyboardMoving)
		{
			HandleKeyboardMovement();
		}

		if (_isMouseRotating)
		{
			HandleRotation();
		}
		else if (IsKeyboardRotating)
		{
			HandleKeyboardRotation();
		}

		ApplyValues();
	}

	void ApplyValues()
	{
		_newZoom.y = Mathf.Clamp(_newZoom.y, -_maxZoomAmount, _maxZoomAmount);
		_newZoom.z = Mathf.Clamp(_newZoom.z, -_maxZoomAmount, _maxZoomAmount);
		_transform.SetPositionAndRotation(
			Vector3.Lerp(_transform.position, _newPosition, Time.unscaledDeltaTime * _smoothingTime),
			Quaternion.Lerp(_transform.rotation, _newRotation, Time.unscaledDeltaTime * _smoothingTime)
		);
		_cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _cameraLocalPosition + _newZoom, Time.unscaledDeltaTime * _smoothingTime);
	}

	void HandleMovement()
	{
		var plane = new Plane(Vector3.up, Vector3.zero);
		var ray = _mainCam.ScreenPointToRay(MousePosition);
		plane.Raycast(ray, out var entry);
		_newPosition = _transform.position + _dragStartPosition - ray.GetPoint(entry);
	}

	void HandleKeyboardMovement()
	{
		var zoomFactor = Mathf.Clamp(1 + (_newZoom.y / _maxZoomAmount), 0.5f, 1.5f);
		var direction = _transform.TransformDirection(new Vector3(KeyboardMovement.x, 0, KeyboardMovement.y));
		_newPosition += _keyboardMovementSpeed * zoomFactor * direction;
	}

	void HandleRotation()
	{
		Vector3 rotateCurrentPosition = MousePosition;
		const float rotationDivisionAmount = 5;
		var difference = _rotateStartPosition - rotateCurrentPosition;
		_rotateStartPosition = rotateCurrentPosition;
		_newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / rotationDivisionAmount));
	}

	void HandleKeyboardRotation() => _newRotation *= Quaternion.Euler(Vector3.up * KeyboardRotation);

	void MoveStart(InputAction.CallbackContext context)
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			// The mouse is over a UI element, do not move the camera
			return;
		}
		var plane = new Plane(Vector3.up, Vector3.zero);
		var ray = _mainCam.ScreenPointToRay(MousePosition);
		plane.Raycast(ray, out var entry);
		_dragStartPosition = ray.GetPoint(entry);
		_isMouseMoving = true;
	}

	void MoveCanceled(InputAction.CallbackContext context) => _isMouseMoving = false;

	void Scroll(InputAction.CallbackContext context) => _newZoom += context.ReadValue<float>() * _zoomVector;

	void RotationStart(InputAction.CallbackContext context)
	{
		_rotateStartPosition = MousePosition;
		_isMouseRotating = true;
	}

	void RotationCanceled(InputAction.CallbackContext context) => _isMouseRotating = false;
	void OnDestroy()
	{
		Controls.Mouse.PrimaryFire.started -= MoveStart;
		Controls.Mouse.PrimaryFire.canceled -= MoveCanceled;
		Controls.Mouse.Rotation.started -= RotationStart;
		Controls.Mouse.Rotation.canceled -= RotationCanceled;
		Controls.Mouse.Scroll.performed -= Scroll;
	}
}
