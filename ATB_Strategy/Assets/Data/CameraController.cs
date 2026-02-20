using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private Transform _baseTransform;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _cameraPitch = 40f;

    [Header("Move settings")]
    [SerializeField] private float _moveSpeedMaxZoom = 5f;
    [SerializeField] private float _moveSpeedMinZoom = 15f;
    [SerializeField] private float _moveThreshold = 0.2f;
    [SerializeField] private float _smoothTimeStart = 0.15f;
    [SerializeField] private float _smoothTimeEnd = 0.05f;
    private Vector3 _moveVelocity;
    private Vector3 _moveSmoothVelocity;
    private Vector3 _targetMoveDirection;


    [Header("Rotation settings")]
    [SerializeField] private float _rotationAngle = -90f;
    [SerializeField] private float _startAngle = 315f;
    [SerializeField] private float _rotationSpeed = 5f;
    private float _currentAngleY;

    [Header("Zoom settings")]
    [SerializeField] private float _maxZoom = -1.5f;
    [SerializeField] private float _minZoom = -10f;
    [SerializeField] private float _zoomSpeed = 0.3f;
    [SerializeField] private float _zoomSmoothTime = 0.15f;
    private float _targetZoomPercent;
    private float _zoomVelocity;

    [Header("Focus settings")]
    [SerializeField] private float _focusMaxDistance = 15f;
    [SerializeField] private float _focusSmoothTime = 0.2f;
    private Vector3 _focusVelocity;
    private Transform _focusTarget;

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

    public void Init(Transform target)
    {
        _baseTransform.localEulerAngles = new Vector3(_cameraPitch, 0, 0);
        transform.eulerAngles = new Vector3(0, _startAngle, 0);
        EnterFocusMode(target, true);
        _currentAngleY = _startAngle;
        _targetZoomPercent = 0f;
        _cameraTransform.localPosition = new Vector3(0, 0, _minZoom);
    }

    private void Update()
    {
        Zoom();

        Rotate();

        Move();
    }

    public void EnterFocusMode(Transform target, bool instantly = false)
    {
        _focusTarget = target;

        if(instantly || Vector3.Distance(transform.position, _focusTarget.position) >= _focusMaxDistance)
        {
            transform.position = _focusTarget.position;
        }
    }

    public void ExitFocusMode()
    {
        _moveVelocity = Vector3.zero;
        _moveSmoothVelocity = Vector3.zero;
        _focusVelocity = Vector3.zero;
        _focusTarget = null;
    }

    private void Move()
    {
        Vector2 input = _inputActions.Camera.Move.ReadValue<Vector2>();

        bool hasInput = input.magnitude >= _moveThreshold;

        if (hasInput && _focusTarget)
        {
            ExitFocusMode();
        }

        if (_focusTarget)
        {
            MoveToTarget();
        }
        else
        {
            MoveToDirection(hasInput ? input : Vector2.zero);
        }
    }

    private void MoveToDirection(Vector2 input)
    {
        if (input == Vector2.zero && _moveVelocity == Vector3.zero)
            return;

        _targetMoveDirection = transform.forward * input.y + transform.right * input.x;

        float moveSpeed = Mathf.Lerp(_moveSpeedMinZoom, _moveSpeedMaxZoom, _targetZoomPercent);
        Vector3 targetVelocity = _targetMoveDirection * moveSpeed;

        float smoothTime = input == Vector2.zero ? _smoothTimeEnd : _smoothTimeStart;

        _moveVelocity = Vector3.SmoothDamp(
            _moveVelocity,
            targetVelocity,
            ref _moveSmoothVelocity,
            smoothTime);


        transform.position += _moveVelocity * Time.deltaTime;
    }

    private void MoveToTarget()
    {
        if (!_focusTarget) return;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            _focusTarget.position,
            ref _focusVelocity,
            _focusSmoothTime);
    }


    private void Zoom()
    {
        float input = _inputActions.Camera.Zoom.ReadValue<float>();

        _targetZoomPercent += input * _zoomSpeed;
        _targetZoomPercent = Mathf.Clamp(_targetZoomPercent, 0, 1);

        float targetZoom = Mathf.Lerp(_minZoom, _maxZoom, _targetZoomPercent);

        float currentZoom = _cameraTransform.localPosition.z;

        float smoothZoom = Mathf.SmoothDamp(
            currentZoom,
            targetZoom,
            ref _zoomVelocity,
            _zoomSmoothTime);

        _cameraTransform.localPosition = new Vector3(0, 0, smoothZoom);
    }

    private void Rotate()
    {
        float yAngle = Mathf.LerpAngle(transform.eulerAngles.y, _currentAngleY, Time.deltaTime * _rotationSpeed);
        Vector3 euler = transform.eulerAngles;
        euler.y = yAngle;
        transform.eulerAngles = euler;
    }

    private void RotateToAngle(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();

        float deltaAngle = Mathf.Sign(value) * _rotationAngle;
        _currentAngleY = Mathf.Repeat(_currentAngleY + deltaAngle, 360f);
    }
}
