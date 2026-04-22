using UnityEngine;
using System.Collections.Generic;

public class UnitCombat : CombatObject
{
    public WeaponController Weapon;
    
    public override CombatBodyParts BodyParts { get => _bodyParts; }
    [SerializeField] private CombatBodyParts _bodyParts;

    public override UnitOwner Owner { get => _unitController.Owner; }

    public override int Dodge { get => _unitController.UnitStats.Dodge; }

    public List<CombatTarget> Targets = new List<CombatTarget>();

    private UnitController _unitController;

    public void Init(UnitController unit)
    {
        _unitController = unit;
    }

    public override void GetDamage(HitResult result)
    {
        if (result.IsCritical)
        {
            if (_unitController.UnitStats.Armor >= result.Damage)
            {
                _unitController.UnitStats.Armor.Value = 0;
            }
            else
            {
                int breakThrowDamage = result.Damage - _unitController.UnitStats.Armor;
                _unitController.UnitStats.Armor.Value = 0;
                _unitController.UnitStats.Health.Value -= Mathf.Clamp(breakThrowDamage, 0,_unitController.UnitStats.Health);
            }
        }
        else
        {
            int breakThrowDamage = result.Damage - _unitController.UnitStats.Armor;
            _unitController.UnitStats.Armor.Value -= Mathf.Clamp(result.Damage, 0, _unitController.UnitStats.Armor);
            _unitController.UnitStats.Health.Value -= Mathf.Clamp(breakThrowDamage, 0,_unitController.UnitStats.Health);
        }
    }
}
