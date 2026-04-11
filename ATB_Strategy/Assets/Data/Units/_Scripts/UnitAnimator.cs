using System;
using System.Collections;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private UnitWeaponIKController _weaponIK;
    [SerializeField] private Transform _unitModel;
    
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

    public event Action OnAttackAnim;

    public void AnimateAim(Vector3 target, float fullTime)
    {
        StartCoroutine(AimAnimation(target, fullTime));
    }

    private IEnumerator AimAnimation(Vector3 target, float fullTime)
    {
        float animTime = TurnManager.TurnTime;
        float delay = TurnManager.TurnTime * 0.2f;

        float rotateTime = (fullTime - animTime - delay * 4f) / 2f;

        Quaternion startRot = _unitModel.rotation;
        TileCover startCover = _cover;

        Vector3 dir = Vector3.ProjectOnPlane(target - _unitModel.position, Vector3.up).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

        // 🔹 Phase 1 — Delay
        yield return Wait(delay);

        // 🔹 Phase 2 — Rotate to target
        yield return RotatePhase(startRot, targetRot, rotateTime, target, 0f, 1f, TileCover.None);

        // 🔹 Phase 3 — Pre-fire delay
        yield return WaitWithAim(delay, target);

        // 🔹 Phase 4 — Fire
        yield return WaitWithAim(animTime, target); // тут потом вставишь анимацию

        // 🔹 Phase 5 — Post-fire delay
        yield return WaitWithAim(delay, target);

        // 🔹 Phase 6 — Rotate back
        yield return RotatePhase(startRot, targetRot, rotateTime, target, 1f, 0f, startCover);

        // 🔹 Phase 7 — End delay
        yield return Wait(delay);
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

    private IEnumerator RotatePhase(
    Quaternion startRotation,
    Quaternion targetRotation,
    float duration,
    Vector3 target,
    float coverFrom,
    float coverTo,
    TileCover coverState)
    {
        float t = 0;

        while (t < duration)
        {
            t += TimeService.TimeSpeedDelta;
            float lerp = Mathf.Clamp01(t / duration);
            float eased = 1f - Mathf.Pow(1f - lerp, 2);

            _unitModel.rotation = Quaternion.Lerp(startRotation, targetRotation, Mathf.Lerp(coverFrom, coverTo, eased));

            // cover плавно
            SetCover(coverState, 0, Mathf.Lerp(coverFrom, coverTo, lerp));

            // IK
            float angle = Quaternion.Angle(_unitModel.rotation, targetRotation);
            float weight = 1 - Mathf.Clamp01(angle / 60f);

            _weaponIK.SetAimWeight(weight);
            _weaponIK.SetAimRotation(target);

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
