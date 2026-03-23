using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private UnitController _unit;

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

        Vector3 directionXZ = Vector3.ProjectOnPlane(movementDirection, Vector3.up).normalized * movementDirection.magnitude;

        SetMovement(directionXZ, movementDirection.y);
        
        /*_animator.SetBool("Climbing", _unit.AgentController.MovementState == MovementState.Climbing);*/
    }

    private void SetMovement(Vector3 directionXZ, float vertacalDirection)
    {
        Vector3 realDirection = transform.InverseTransformDirection(directionXZ);

        _animator.SetFloat("MoveX", realDirection.x);
        _animator.SetFloat("MoveZ", realDirection.z);

        _animator.SetFloat("MoveVertical", vertacalDirection);
    }
    private void UpdateAnimationSpeed(float timeSpeed)
    {
        _animator.speed = timeSpeed;
    }
}
