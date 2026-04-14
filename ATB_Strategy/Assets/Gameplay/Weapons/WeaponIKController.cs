using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponIKController : MonoBehaviour
{
    [Header("Rig settings")]
    [SerializeField] private MultiAimConstraint _headAimConstraint;
    [SerializeField] private MultiAimConstraint _weaponAimConstraint;
    [SerializeField] private MultiPositionConstraint _weaponPositionConstraint;

    [Header("Aim Settings")]
    [SerializeField] private Transform _aimTransform;

    private void Awake()
    {
        SetAimWeight(0f);
    }

    public void SetAimWeight(float value)
    {
        value = Mathf.Clamp01(value);

        _headAimConstraint.weight = value;

        var source = _weaponAimConstraint.data.sourceObjects;
        source.SetWeight(0, 1 - value);
        source.SetWeight(1, value);
        _weaponAimConstraint.data.sourceObjects = source;

        source = _weaponPositionConstraint.data.sourceObjects;
        source.SetWeight(0, 1 - value);
        source.SetWeight(1, value);
        _weaponPositionConstraint.data.sourceObjects = source;
    }

    public void SetAimRotation(Vector3 target)
    {
        _aimTransform.LookAt(target);
    }
}
