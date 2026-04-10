using UnityEngine;
using UnityEngine.Animations.Rigging;

public class UnitWeaponIKController : MonoBehaviour
{
    [SerializeField] private MultiAimConstraint _headAimConstraint;
    [SerializeField] private MultiAimConstraint _weaponAimConstraint;
    [SerializeField] private MultiPositionConstraint _weaponPositionConstraint;

    private void Awake()
    {
        SetAimWeight(1f);
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
}
