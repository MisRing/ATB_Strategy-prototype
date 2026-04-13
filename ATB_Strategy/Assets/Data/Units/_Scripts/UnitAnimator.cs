using System;
using System.Collections;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private UnitWeaponIKController _weaponIK;
    [SerializeField] private Transform _unitModel;

    [SerializeField] private Animator _weaponAnimator;


    private static readonly int MOVEMENT_X_ID = Animator.StringToHash("MoveX");
    private static readonly int MOVEMENT_Z_ID = Animator.StringToHash("MoveZ");
    
    private static readonly int COVER_VERTICAL_ID = Animator.StringToHash("CoverVertical");
    private static readonly int COVER_HORIZONTAL_ID = Animator.StringToHash("CoverHorizontal");

    private static readonly int AIM_ID = Animator.StringToHash("Aim");
    private static readonly int SHOOT_ID = Animator.StringToHash("Shoot");


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

    public event Action OnAttackAnim;

    public void AnimateAim(Vector3 target, float fullTime)
    {
        StartCoroutine(AimAnimation(target, fullTime));
    }

    private IEnumerator AimAnimation(Vector3 target, float fullTime)
    {
        float t1 = fullTime * 0.05f;
        float t2 = fullTime * 0.25f;
        float t3 = fullTime * 0.1f;
        float t4 = fullTime * 0.15f;
        float t5 = fullTime * 0.05f;
        float t6 = fullTime * 0.35f;
        float t7 = fullTime * 0.05f;


        Quaternion startRot = _unitModel.rotation;
        TileCover startCover = _cover;

        Vector3 dir = Vector3.ProjectOnPlane(target - _unitModel.position, Vector3.up).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

        // 🔹 Phase 1 — Delay
        yield return Wait(t1);

        _animator.SetBool(AIM_ID, true);

        // 🔹 Phase 2 — Rotate to target
        //yield return RotatePhase(startRot, targetRot, t2, target, 0f, 1f, TileCover.None);
        yield return RotateToTarget(startRot, targetRot, t2, TileCover.None, target, 0f, 1f);

        // 🔹 Phase 3 — Pre-fire delay
        yield return WaitWithAim(t3, target);

        _animator.SetTrigger(SHOOT_ID);
        _weaponAnimator.SetTrigger("Fire");
        // 🔹 Phase 4 — Fire
        yield return WaitWithAim(t4, target); // тут потом вставишь анимацию

        // 🔹 Phase 5 — Post-fire delay
        yield return WaitWithAim(t5, target);

        _animator.SetBool(AIM_ID, false);
        // 🔹 Phase 6 — Rotate back
        if (startCover == 0)
        {
            transform.rotation = targetRot;
            _unitModel.transform.rotation = targetRot;
            startRot = targetRot;
        }
        yield return RotateToTarget(startRot, targetRot, t6, startCover, Vector3.zero, 1f, 0f);


        // 🔹 Phase 7 — End delay
        yield return Wait(t7);
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
        _animator.speed = 1f;// timeSpeed;
    }

    private TileCover _cover;
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
        _cover = cover;
    }
}
