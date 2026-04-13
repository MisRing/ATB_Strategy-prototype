using System;
using System.Collections;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private UnitWeaponIKController _weaponIK;
    [SerializeField] private Transform _unitModel;

    [SerializeField] private Animator _weaponAnimator;

    private TileCover _coverState;

    public static readonly int MOVEMENT_X_ID = Animator.StringToHash("MoveX");
    public static readonly int MOVEMENT_Z_ID = Animator.StringToHash("MoveZ");
    
    public static readonly int COVER_VERTICAL_ID = Animator.StringToHash("CoverVertical");
    public static readonly int COVER_HORIZONTAL_ID = Animator.StringToHash("CoverHorizontal");

    public static readonly int AIM_ID = Animator.StringToHash("Aim");
    public static readonly int SHOOT_ID = Animator.StringToHash("Shoot");
    
    private UnitController _unit;

    private void Awake()
    {
        UpdateAnimationSpeed(TimeService.TimeSpeed);
    }

    public void Init(UnitController unit)
    {
        _unit = unit;
        // TODO: MOVE TO OnEnable();
        TimeService.OnTimeSpeedChanged += UpdateAnimationSpeed;
        _unit.AgentController.OnCoverChanged += SetCover;
    }
    
    private void OnEnable()
    {
        // TimeService.OnTimeSpeedChanged += UpdateAnimationSpeed;
        // _unit.AgentController.OnCoverChanged += SetCover;
    }
    
    private void OnDisable()
    {
        TimeService.OnTimeSpeedChanged -= UpdateAnimationSpeed;
        _unit.AgentController.OnCoverChanged -= SetCover;
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

    public event Action OnAttackAnim;

    private Quaternion _aimStartRotation;
    private Quaternion _aimTargetRotation;
    private TileCover _coverBeforeAim;
    public IEnumerator Aim(float duration, Vector3 target)
    {
        _aimStartRotation = transform.rotation;
        Vector3 dir = Vector3.ProjectOnPlane(target - transform.position, Vector3.up).normalized;
        _aimTargetRotation = Quaternion.LookRotation(dir, Vector3.up);
        _coverBeforeAim = _coverState;
        
        float waitDelay = duration * 0.2f;
        float rotateDuration = duration * 0.8f;
            
        yield return Wait(waitDelay);
    
        _animator.SetBool(AIM_ID, true);
        yield return RotateToTarget(_aimStartRotation, _aimTargetRotation, rotateDuration, TileCover.None, target, 0f, 1f);
    }
    
    public IEnumerator Shoot(float duration, Vector3 target, bool shoot = true)
    {
        float aimDuration = duration * 0.35f;
        float shootDuration = duration * 0.5f;
        float waitDelay = duration * 0.15f;
        
        yield return WaitWithAim(aimDuration, target);
        if (shoot)
        {
            _animator.SetTrigger(SHOOT_ID);
            _weaponAnimator.SetTrigger("Fire");
        }
        yield return WaitWithAim(shootDuration, target); // тут потом вставишь анимацию
        yield return WaitWithAim(waitDelay, target);
    }
    
    public IEnumerator EndAim(float duration)
    {
        float rotateDuration = duration * 0.9f;
        float waitDelay = duration * 0.1f;
        if (_coverState == 0)
        {
            transform.rotation = _aimTargetRotation;
            _unitModel.transform.rotation = _aimTargetRotation;
            _aimStartRotation = _aimTargetRotation;
        }
        _animator.SetBool(AIM_ID, false);
        yield return RotateToTarget(_aimStartRotation, _aimTargetRotation, rotateDuration, _coverBeforeAim, Vector3.zero, 1f, 0f);
        yield return Wait(waitDelay);
    }

    private IEnumerator Wait(float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += TimeService.TimeSpeedDelta;
            yield return null;
        }
    }

    private IEnumerator WaitWithAim(float duration, Vector3 target)
    {
        float t = 0;
        while (t < duration)
        {
            t += TimeService.TimeSpeedDelta;

            _weaponIK.SetAimRotation(target);

            yield return null;
        }
    }

    private IEnumerator RotateToTarget(
        Quaternion startRotation,
        Quaternion targetRotation,
        float duration,
        TileCover endCover,
        Vector3 target,
        float from,
        float to)
    {
        float t = 0;

        float angle = Quaternion.Angle(startRotation, targetRotation);
        bool angleWeight = angle > 60f;


        while (t < duration)
        {
            t += TimeService.TimeSpeedDelta;
            float lerp = Mathf.Clamp01(t / duration);
            float eased = 1f - Mathf.Pow(1f - lerp, 2);
            _unitModel.rotation = Quaternion.Lerp(startRotation, targetRotation, Mathf.Lerp(from, to, eased));
            SetCover(endCover, 0, eased);
            if (angleWeight)
            {
                angle = Quaternion.Angle(_unitModel.rotation, targetRotation);
                float weight = 1f - Mathf.Clamp01(angle / 60f);
                weight = Mathf.Pow(weight, 2);
                
                _weaponIK.SetAimWeight(weight);
            }
            else
            {
                _weaponIK.SetAimWeight(Mathf.Lerp(from, to, eased));
            }
            if (target != Vector3.zero)
            {
                _weaponIK.SetAimRotation(target);
            }

            yield return null;
        }
    }

    private void UpdateAnimationSpeed(float timeSpeed)
    {
        _animator.speed = timeSpeed;
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
