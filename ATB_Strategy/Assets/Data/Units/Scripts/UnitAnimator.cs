using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private UnitController _unit;

    [SerializeField] private float _coverAnimationSpeed = 1f;
    private bool _cover;
    [SerializeField] private float _coverVelocity = 0;

    private void Awake()
    {
        UpdateAnimationSpeed(TimeService.TimeSpeed);
    }

    private void OnEnable()
    {
        TimeService.OnTimeSpeedChanged += UpdateAnimationSpeed;
    }
    
    private void OnDisable()
    {
        TimeService.OnTimeSpeedChanged -= UpdateAnimationSpeed;
    }

    public void Init(UnitController unit)
    {
        _unit = unit;
    }

    private void Update()
    {
        Vector3 movementDirection = _unit.AgentController.Velocity;
        SetMovement(movementDirection.x, movementDirection.z);
    }

    private void SetMovement(float directionX, float directionZ)
    {
        Vector3 direction = new Vector3(directionX, 0f, directionZ);
        Vector3 realDirection = transform.InverseTransformDirection(direction);

        _animator.SetFloat("MoveX", realDirection.x);
        _animator.SetFloat("MoveZ", realDirection.z);
    }

    public void SetCover(bool cover)
    {
        _cover = cover;
    }

    //private void Update()
    //{
    //    if (_cover && _coverVelocity < 1)
    //    {
    //        _coverVelocity += TimeService.TimeSpeedDelta * _coverAnimationSpeed;
    //        _coverVelocity = Mathf.Clamp01(_coverVelocity);
    //    }
    //    else if (!_cover && _coverVelocity > 0)
    //    {
    //        _coverVelocity -= TimeService.TimeSpeedDelta * _coverAnimationSpeed;
    //        _coverVelocity = Mathf.Clamp01(_coverVelocity);
    //    }
        
    //    _animator.SetFloat("Cover", _coverVelocity);
    //}

    private void UpdateAnimationSpeed(float timeSpeed)
    {
        _animator.speed = timeSpeed;
    }
}
