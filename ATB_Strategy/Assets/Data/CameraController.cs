using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class CameraController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private Transform _baseTransform;
    [SerializeField] private Transform _cameraTransform;

    [Header("Rotation settings")]
    [SerializeField] private float _rotationAngle = -90f;
    [SerializeField] private float _startAngle = 315f;
    [SerializeField] private float _rotationSpeed = 5f;
    private float _currentAngleY;

    [Header("Zoom settings")]
    [SerializeField] private float _maxZoom = -1.5f;
    [SerializeField] private float _minZoom = -10f;
    [SerializeField] private float _zoomSpeed = 0.3f;
    [SerializeField] private float _smoothTime = 0.15f;
    private float _targetZoom;
    private float _zoomVelocity;

    private InputActions _inputActions;

    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();

        _inputActions.Camera.Rotate.started += RotateToAngle;
    }

    private void OnDisable()
    {
        _inputActions.Disable();

        _inputActions.Camera.Rotate.started -= RotateToAngle;
    }

    public void Init(Vector3 startPosition)
    {
        _baseTransform.eulerAngles = new Vector3(40f, _startAngle, 0);
        transform.position = startPosition;
        _currentAngleY = _startAngle;
        _targetZoom = _cameraTransform.localPosition.z;
    }

    private void RotateToAngle(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();

        float deltaAngle = Mathf.Sign(value) * _rotationAngle;
        _currentAngleY = Mathf.Repeat(_currentAngleY + deltaAngle, 360f);
    }

    private void Update()
    {
        Zoom();

        Rotate();
    }

    private void Zoom()
    {
        float input = _inputActions.Camera.Zoom.ReadValue<float>();

        _targetZoom += input * _zoomSpeed;
        _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);

        float currentZoom = _cameraTransform.localPosition.z;

        float smoothZoom = Mathf.SmoothDamp(
            currentZoom,
            _targetZoom,
            ref _zoomVelocity,
            _smoothTime);

        _cameraTransform.localPosition = new Vector3(0, 0, smoothZoom);
    }

    private void Rotate()
    {
        float yAngle = Mathf.LerpAngle(_baseTransform.eulerAngles.y, _currentAngleY, Time.deltaTime * _rotationSpeed);
        _baseTransform.eulerAngles = new Vector3(_baseTransform.eulerAngles.x,
                                                            yAngle,
                                                            _baseTransform.eulerAngles.z);
    }
}
