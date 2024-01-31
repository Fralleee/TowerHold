using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : Controller
{
	[SerializeField] Transform _cameraTransform;

	[Header("Movement Settings")]
	[SerializeField] float _keyboardMovementSpeed = 0.25f;
	[SerializeField] float _mouseMovementSpeed = 0.25f;
	[SerializeField] float _edgeTolerancePercent = 15;
	[SerializeField] float _maxDistanceFromCenter = 120;

	[Header("Zoom Settings")]
	[SerializeField] float _zoomAmount = 10;
	[SerializeField] float _minZoomAmount = 50;
	[SerializeField] float _maxZoomAmount = 150;

	[Header("Rotation Settings")]
	[SerializeField] float _snapRotationDegrees = 45;

	[Header("Animation Settings")]
	[SerializeField] float _smoothingTime = 10;

	[Header("Cursor Settings")]
	[SerializeField] Texture2D _moveCursor;
	[SerializeField] Texture2D _rotationCursor;

	Camera _mainCam;
	Transform _transform;
	Vector3 _zoomVector;
	Vector3 _newPosition;
	Vector3 _newZoom;
	Vector3 _dragStartPosition;
	Vector3 _rotateStartPosition;
	Vector3 _cameraLocalPosition;
	Quaternion _newRotation;
	Vector3 _startPosition;
	Vector3 _cameraStartLocalPosition;
	Quaternion _startRotation;
	bool _isMouseMoving;
	bool _isMouseRotating;
	bool IsKeyboardMoving => KeyboardMovement.magnitude != 0;
	bool IsKeyboardRotating => KeyboardRotation != 0;

	void Start()
	{
		_transform = transform;
		_mainCam = _cameraTransform.GetComponentInChildren<Camera>();

		Controls.Mouse.PanStart.started += MoveStart;
		Controls.Mouse.PanStart.canceled += MoveCanceled;
		Controls.Mouse.Rotation.started += RotationStart;
		Controls.Mouse.Rotation.canceled += RotationCanceled;
		Controls.Mouse.Scroll.performed += Scroll;

		_zoomVector = new Vector3(0, -_zoomAmount, _zoomAmount);

		_newPosition = _transform.position;
		_newRotation = _transform.rotation;
		_cameraLocalPosition = _cameraTransform.localPosition;

		_startPosition = _transform.position;
		_startRotation = _transform.rotation;
		_cameraStartLocalPosition = _cameraTransform.localPosition;
	}

	void LateUpdate()
	{
		if (_isMouseMoving)
		{
			Cursor.SetCursor(_moveCursor, new Vector2(12, 12), CursorMode.Auto);
			HandleMovement();
		}
		else if (IsKeyboardMoving)
		{
			HandleKeyboardMovement();
		}
		else if (_isMouseRotating)
		{
			Cursor.SetCursor(_rotationCursor, new Vector2(12, 12), CursorMode.Auto);
			HandleRotation();
		}
		else if (IsKeyboardRotating)
		{
			HandleKeyboardRotation();
		}
		else
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			CheckMouseAtScreenEdge();
			SnapRotation();
		}

		if (KeyboardReset)
		{
			ResetCamera();
		}

		ApplyValues();
	}

	void ApplyValues()
	{
		_newZoom.y = Mathf.Clamp(_newZoom.y, -_minZoomAmount, _maxZoomAmount);
		_newZoom.z = Mathf.Clamp(_newZoom.z, -_maxZoomAmount, _minZoomAmount);

		var maxDistanceWithZoom = _maxDistanceFromCenter - (_newZoom.y * 0.3f);
		if (Vector3.Distance(_newPosition, Vector3.zero) > maxDistanceWithZoom)
		{
			_newPosition = _newPosition.normalized * maxDistanceWithZoom;
		}

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
		_ = plane.Raycast(ray, out var entry);
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


	void CheckMouseAtScreenEdge()
	{
		var mousePosition = MousePosition;
		var screenSize = new Vector2(Screen.width, Screen.height);

		// If mouse position is outside of the application window, do not move the camera
		if (mousePosition.x < 0 || mousePosition.x > screenSize.x || mousePosition.y < 0 || mousePosition.y > screenSize.y)
		{
			return;
		}

		if (mousePosition.x < _edgeTolerancePercent * Screen.width / 100)
		{
			_newPosition -= transform.right * _mouseMovementSpeed;
		}
		else if (mousePosition.x > screenSize.x - (_edgeTolerancePercent * Screen.width / 100))
		{
			_newPosition += transform.right * _mouseMovementSpeed;
		}

		if (mousePosition.y < _edgeTolerancePercent * Screen.height / 100)
		{
			_newPosition -= transform.forward * _mouseMovementSpeed;
		}
		else if (mousePosition.y > screenSize.y - (_edgeTolerancePercent * Screen.height / 100))
		{
			_newPosition += transform.forward * _mouseMovementSpeed;
		}
	}

	void SnapRotation()
	{
		var rotation = _newRotation.eulerAngles;
		rotation.y = Mathf.Round(rotation.y / _snapRotationDegrees) * _snapRotationDegrees;
		_newRotation.eulerAngles = rotation;
	}

	void ResetCamera()
	{
		_newPosition = _startPosition;
		_newRotation = _startRotation;
		_cameraLocalPosition = _cameraStartLocalPosition;
		_newZoom = Vector3.zero;
	}

	void MoveStart(InputAction.CallbackContext context)
	{
		// if (EventSystem.current.IsPointerOverGameObject())
		// {
		// 	// The mouse is over a UI element, do not move the camera
		// 	return;
		// }
		var plane = new Plane(Vector3.up, Vector3.zero);
		var ray = _mainCam.ScreenPointToRay(MousePosition);
		_ = plane.Raycast(ray, out var entry);
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
