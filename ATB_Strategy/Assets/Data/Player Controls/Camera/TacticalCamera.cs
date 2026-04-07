using UnityEngine;

public class TacticalCamera : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private float _cameraPitch = 40f;
    [SerializeField] private float _transitionMoveSmooth = 0.25f;
    [SerializeField] private float _transitionRotSpeed = 5f;

    private bool _isTransitioning;
    private Vector3 _transitionVelocity;
    
    private Vector3 _position;
    private Vector3 _rotation;
    private float _zoom;

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
    [SerializeField] private float _maxZoom = 1.5f;
    [SerializeField] private float _minZoom = 10f;
    [SerializeField] private float _zoomSpeed = 0.3f;
    [SerializeField] private float _zoomSmoothTime = 0.15f;
    [SerializeField] private float _extraZoomPercent = 1.2f;
    private bool _extraZoom = false;
    private float _targetZoomPercent;
    private float _zoomVelocity;

    [Header("Focus settings")]
    [SerializeField] private float _focusMaxDistance = 15f;
    [SerializeField] private float _focusSmoothTime = 0.2f;
    private Vector3 _focusVelocity;
    private Transform _focusTarget;

    private void OnEnable()
    {
        PlayerInputController.RotateCamera += RotateToAngle;
        _isTransitioning = true;
    }

    private void OnDisable()
    {
        PlayerInputController.RotateCamera -= RotateToAngle;
    }

    public void Init(Transform target)
    {
        _rotation = new Vector3(0, _startAngle, 0);
        EnterFocusMode(target, true);
        _currentAngleY = _startAngle;
        _targetZoomPercent = 0f;
        _zoom = _minZoom;
        SetPosition();
    }

    private void Update()
    {
        if (_isTransitioning)
        {
            UpdateTransition();
            return;
        }
        
        Zoom();
        Rotate();
        Move();
        SetPosition();
    }
    
    private void UpdateTransition()
    {
        Quaternion targetRot = Quaternion.Euler(_cameraPitch, _rotation.y, 0f);

        Vector3 offset = targetRot * Vector3.back;
        Vector3 targetPos = _position + offset * _zoom;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref _transitionVelocity,
            _transitionMoveSmooth
            );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * _transitionRotSpeed
            );

        bool posDone = Vector3.Distance(transform.position, targetPos) < 0.05f;
        bool rotDone = Quaternion.Angle(transform.rotation, targetRot) < 0.5f;

        if (posDone && rotDone)
        {
            transform.position = targetPos;
            transform.rotation = targetRot;

            _isTransitioning = false;
        }
    }

    private void SetPosition()
    {
        Quaternion targetRotation = Quaternion.Euler(_cameraPitch, _rotation.y, 0f);

        Vector3 offset = targetRotation * Vector3.back;

        Vector3 targetPosition = _position + offset * _zoom;

        transform.rotation = targetRotation;
        transform.position = targetPosition;
    }

    public void EnterFocusMode(Transform target, bool instantly = false)
    {
        _focusTarget = target;

        if(instantly || Vector3.Distance(_position, _focusTarget.position) >= _focusMaxDistance)
        {
            _position = _focusTarget.position;
        }
    }

    public void ExitFocusMode()
    {
        _moveVelocity = Vector3.zero;
        _moveSmoothVelocity = Vector3.zero;
        _focusVelocity = Vector3.zero;
        _focusTarget = null;
    }

    public void SetExtraZoom(bool value) => _extraZoom = value;

    private void Move()
    {
        Vector2 input = PlayerInputController.CameraMoveAxis;

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

        Vector3 forward = Quaternion.Euler(_rotation) * Vector3.forward;
        Vector3 right = Quaternion.Euler(_rotation) * Vector3.right;
        _targetMoveDirection = forward * input.y + right * input.x;

        float moveSpeed = Mathf.Lerp(_moveSpeedMinZoom, _moveSpeedMaxZoom, _targetZoomPercent);
        Vector3 targetVelocity = _targetMoveDirection * moveSpeed;

        float smoothTime = input == Vector2.zero ? _smoothTimeEnd : _smoothTimeStart;

        _moveVelocity = Vector3.SmoothDamp(
            _moveVelocity,
            targetVelocity,
            ref _moveSmoothVelocity,
            smoothTime);


        _position += _moveVelocity * Time.deltaTime;
    }

    private void MoveToTarget()
    {
        if (!_focusTarget) return;

        _position = Vector3.SmoothDamp(
            _position,
            _focusTarget.position,
            ref _focusVelocity,
            _focusSmoothTime);
    }
    
    private void Zoom()
    {
        float input = PlayerInputController.CameraZoomAxis;

        _targetZoomPercent += input * _zoomSpeed;
        _targetZoomPercent = Mathf.Clamp(_targetZoomPercent, 0, 1);

        float targetZoom = Mathf.Lerp(_minZoom, _maxZoom, _targetZoomPercent);
        
        if (_extraZoom)
        {
            targetZoom /= _extraZoomPercent;
        }
        
        float currentZoom = _zoom;

        float smoothZoom = Mathf.SmoothDamp(
            currentZoom,
            targetZoom,
            ref _zoomVelocity,
            _zoomSmoothTime);

        _zoom = smoothZoom;
    }

    private void Rotate()
    {
        float yAngle = Mathf.LerpAngle(_rotation.y, _currentAngleY, Time.deltaTime * _rotationSpeed);
        Vector3 euler = _rotation;
        euler.y = yAngle;
        _rotation = euler;
    }

    private void RotateToAngle(float value)
    {
        if (_isTransitioning) return;
        
        float deltaAngle = Mathf.Sign(value) * _rotationAngle;
        _currentAngleY = Mathf.Repeat(_currentAngleY + deltaAngle, 360f);
    }
}