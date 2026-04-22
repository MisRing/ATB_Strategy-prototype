using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon stats")]
    public RangeIntStat Damage = new RangeIntStat(1, 3);
    public IntStat CritChance = new IntStat(10);
    public RangeIntStat CritDamage = new RangeIntStat(4, 6);
    public IntStat Accuracy = new IntStat(20);
    public WeaponRangeType RangeType = WeaponRangeType.Medium;
}

public enum WeaponRangeType
{
    Any,
    Melee,
    Medium,
    Ranged,
}