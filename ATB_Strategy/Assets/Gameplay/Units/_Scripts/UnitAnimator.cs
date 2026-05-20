using System;
using System.Collections;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private WeaponIKController _weaponIK;
    [SerializeField] private Transform _unitModel;

    [SerializeField] private WeaponAnimator _weaponAnimator;

    private TileCover _coverState;
    private int _coverLook;
    
    //Aim start parameters
    private Quaternion _aimStartRotation;
    private TileCover _coverBeforeAim;
    private int _coverLookBeforeAim;
    
    //Shooting time rations
    private float _aimWaitRatio = 0.2f;
    private float _aimRotateRatio = 0.8f;
    
    private float _shootingStartWaitRatio = 0.35f;
    private float _shootingRatio = 0.5f;
    private float _shootingEndWaitRatio = 0.15f;
    
    private float _endRotateRatio = 0.9f;
    private float _endWaitRatio = 0.1f;

    public static readonly int MOVEMENT_X_ID = Animator.StringToHash("MoveX");
    public static readonly int MOVEMENT_Z_ID = Animator.StringToHash("MoveZ");
    
    public static readonly int COVER_VERTICAL_ID = Animator.StringToHash("CoverVertical");
    public static readonly int COVER_HORIZONTAL_ID = Animator.StringToHash("CoverHorizontal");

    public static readonly int AIM_ID = Animator.StringToHash("Aim");
    public static readonly int SHOOT_ID = Animator.StringToHash("Shoot");
    
    public static readonly int DEATH_ID = Animator.StringToHash("Death");
    public static readonly int HIT_X_ID = Animator.StringToHash("HitX");
    public static readonly int HIT_Z_ID = Animator.StringToHash("HitZ");

    
    private UnitController _unit;

    public void Init(UnitController unit)
    {
        _unit = unit;
    }

    private void OnEnable()
    {
        TimeService.OnTimeSpeedChanged += UpdateAnimationSpeed;
        _unit.Agent.OnCoverChanged += SetCover;
    }
    
    private void OnDisable()
    {
        TimeService.OnTimeSpeedChanged -= UpdateAnimationSpeed;
        _unit.Agent.OnCoverChanged -= SetCover;
    }

    private void Start()
    {
        UpdateAnimationSpeed(TimeService.TimeSpeed);
    }

    private void Update()
    {
        Vector3 movementDirection = _unit.Agent.Velocity;

        Vector3 directionXZ = Vector3.ProjectOnPlane(movementDirection, Vector3.up);
        directionXZ = directionXZ.normalized * movementDirection.magnitude;

        SetMovement(directionXZ);
    }

    private void SetMovement(Vector3 directionXZ)
    {
        Vector3 realDirection = transform.InverseTransformDirection(directionXZ);

        _animator.SetFloat(MOVEMENT_X_ID, realDirection.x);
        _animator.SetFloat(MOVEMENT_Z_ID, realDirection.z);
    }
    
    public IEnumerator Aim(float duration, Transform target)
    {
        _aimStartRotation = transform.rotation;
        _coverBeforeAim = _coverState;
        _coverLookBeforeAim = _coverLook;
        
        float waitDelay = duration * _aimWaitRatio;
        float rotateDuration = duration * _aimRotateRatio;
            
        yield return Wait(waitDelay);
    
        _animator.SetBool(AIM_ID, true);
        yield return RotateToTarget(
            _aimStartRotation,
            target,
            rotateDuration,
            TileCover.None,
            0,
            0f,
            1f,
            true
            );
    }
    
    public IEnumerator Shoot(float duration, Transform target, bool hit, bool shoot)
    {
        float aimDuration = duration * _shootingStartWaitRatio;
        float shootDuration = duration * _shootingRatio;
        float waitDelay = duration * _shootingEndWaitRatio;
        
        yield return WaitWithAim(aimDuration, target);
        if (shoot)
        {
            _animator.SetTrigger(SHOOT_ID);
            _weaponAnimator.FireAnimation(target, hit);
        }
        yield return WaitWithAim(shootDuration, target); // for animation time
        yield return WaitWithAim(waitDelay, target);
    }
    
    public IEnumerator EndAim(float duration, Transform target)
    {
        float rotateDuration = duration * _endRotateRatio;
        float waitDelay = duration * _endWaitRatio;
        
        if (_coverBeforeAim == 0)
        {
            transform.rotation = _unitModel.transform.rotation;
            _unitModel.transform.localRotation = Quaternion.identity;
            _aimStartRotation = transform.rotation;
        }
        _animator.SetBool(AIM_ID, false);
        
        yield return RotateToTarget(
            _aimStartRotation, 
            target, rotateDuration,
            _coverBeforeAim, 
            _coverLookBeforeAim, 
            1f, 
            0f, 
            false
            );
        
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

    private IEnumerator WaitWithAim(float duration, Transform target)
    {
        float t = 0;
        while (t < duration)
        {
            t += TimeService.TimeSpeedDelta;

            _weaponIK.SetAimRotation(target.position);

            yield return null;
        }
    }

    private IEnumerator RotateToTarget(
        Quaternion startRotation,
        Transform target,
        float duration,
        TileCover endCover,
        int coverLook,
        float from,
        float to,
        bool isAiming)
    {
        float t = 0;

        Vector3 dir = Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up).normalized;
        Quaternion aimTargetRotation = Quaternion.LookRotation(dir, Vector3.up);
        
        float angle = Quaternion.Angle(startRotation, aimTargetRotation);
        bool angleWeight = angle > 60f;

        while (t < duration)
        {
            dir = Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up).normalized;
            aimTargetRotation = Quaternion.LookRotation(dir, Vector3.up);
            
            t += TimeService.TimeSpeedDelta;
            float lerp = Mathf.Clamp01(t / duration);
            float eased = 1f - Mathf.Pow(1f - lerp, 2);
            
            _unitModel.rotation = Quaternion.Lerp(startRotation, aimTargetRotation, Mathf.Lerp(from, to, eased));
            
            SetCover(endCover, coverLook, eased);
            
            if (angleWeight)
            {
                angle = Quaternion.Angle(_unitModel.rotation, aimTargetRotation);
                float weight = 1f - Mathf.Clamp01(angle / 60f);
                weight = Mathf.Pow(weight, 2);
                
                _weaponIK.SetAimWeight(weight);
            }
            else
            {
                _weaponIK.SetAimWeight(Mathf.Lerp(from, to, eased));
            }
            if (isAiming)
            {
                _weaponIK.SetAimRotation(target.position);
            }

            yield return null;
        }
    }

    private void UpdateAnimationSpeed(float timeSpeed)
    {
        _animator.speed = timeSpeed * (_unit.Stats.Speed / _unit.Stats.Speed.ClearValue);
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
        _coverState = cover;
        _coverLook = look;
    }

    public void DeathAnimation(Vector3 damageFrom)
    {
        Vector3 damageDirection = damageFrom - transform.position;
        damageDirection = new Vector3(damageDirection.x, 0f, damageDirection.z).normalized;
        
        Vector3 localDir = transform.InverseTransformDirection(damageDirection);

        float x = localDir.x;
        float z = localDir.z;

        float hitX = 0;
        float hitZ = 0;

        if (Mathf.Abs(x) > Mathf.Abs(z))
        {
            hitX = x > 0 ? 1 : -1;
        }
        else
        {
            hitZ = z > 0 ? 1 : -1;
        }

        _animator.SetFloat(HIT_X_ID, hitX);
        _animator.SetFloat(HIT_Z_ID, hitZ);
        
        _animator.SetTrigger(DEATH_ID);
    }
}
