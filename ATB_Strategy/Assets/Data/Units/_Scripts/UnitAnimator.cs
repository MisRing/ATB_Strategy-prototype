using System.Collections;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    private static readonly int MOVEMENT_X_ID = Animator.StringToHash("MoveX");
    private static readonly int MOVEMENT_Z_ID = Animator.StringToHash("MoveZ");
    
    private static readonly int COVER_VERTICAL_ID = Animator.StringToHash("CoverVertical");
    private static readonly int COVER_HORIZONTAL_ID = Animator.StringToHash("CoverHorizontal");

    private UnitController _unit;

    private void Awake()
    {
        UpdateAnimationSpeed(TimeService.TimeSpeed);
    }

    private void OnEnable()
    {
        TimeService.OnTimeSpeedChanged += UpdateAnimationSpeed;
        _unit.AgentController.OnCoverChanged += SetCover;
    }
    
    private void OnDisable()
    {
        TimeService.OnTimeSpeedChanged -= UpdateAnimationSpeed;
        _unit.AgentController.OnCoverChanged -= SetCover;
    }

    public void Init(UnitController unit)
    {
        _unit = unit;
    }

    private void Update()
    {
        Vector3 movementDirection = _unit.AgentController.Velocity;

        Vector3 directionXZ = Vector3.ProjectOnPlane(movementDirection, Vector3.up).normalized * movementDirection.magnitude;

        SetMovement(directionXZ);
    }

    private void SetMovement(Vector3 directionXZ)
    {
        Vector3 realDirection = transform.InverseTransformDirection(directionXZ);

        _animator.SetFloat(MOVEMENT_X_ID, realDirection.x);
        _animator.SetFloat(MOVEMENT_Z_ID, realDirection.z);
    }
    private void UpdateAnimationSpeed(float timeSpeed)
    {
        _animator.speed = 1f;// timeSpeed;
    }

    private void SetCover(TileCover cover, int look, float percent)
    {
        if (cover == TileCover.None)
        {
            _animator.SetFloat(COVER_VERTICAL_ID, Mathf.Lerp(_animator.GetFloat(COVER_VERTICAL_ID), 0f, percent));
            _animator.SetFloat(COVER_HORIZONTAL_ID, Mathf.Lerp(_animator.GetFloat(COVER_HORIZONTAL_ID), 0f, percent));
        }
        else
        {
            float vertical = cover == TileCover.Full ? 1f : -1f;
            float horizontal = look < 0 ? -1f : (look > 0 ? 1f : 0f);
            
            _animator.SetFloat(COVER_VERTICAL_ID, Mathf.Lerp(_animator.GetFloat(COVER_VERTICAL_ID), vertical, percent));
            _animator.SetFloat(COVER_HORIZONTAL_ID, Mathf.Lerp(_animator.GetFloat(COVER_HORIZONTAL_ID), horizontal, percent));
        }
    }
}
