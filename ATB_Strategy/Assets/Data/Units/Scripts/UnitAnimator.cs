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

    public void SetMovement(float directionX, float directionZ)
    {
        Vector3 direction = new Vector3(directionX, 0f, directionZ);
        Vector3 realDirection = transform.InverseTransformDirection(direction);

        _animator.SetFloat("MoveX", realDirection.x);
        _animator.SetFloat("MoveZ", realDirection.z);
    }

    private void UpdateAnimationSpeed(float timeSpeed)
    {
        _animator.speed = timeSpeed;
    }
}
