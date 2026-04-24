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

    public void Start()
    {
        SetVisibility();
    }

    public void SetVisibility()
    {
        Targets = CombatService.GetCombats(this, _unitController.UnitStats.VisionRange, _unitController.Owner);
    }

    public int CheckTarget(UnitController unit)
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            if (Targets[i].Target.gameObject.layer == LayerMask.NameToLayer("Unit"))
            {
                if (Targets[i].Target.gameObject.GetComponent<UnitController>() == unit)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public override void GetDamage(HitResult result)
    {
        if (result.IsCritical)
        {
            if (_unitController.UnitStats.Armor >= result.Damage)
            {
                _unitController.UnitStats.Armor.ClearValue = 0;
            }
            else
            {
                int breakThrowDamage = result.Damage - _unitController.UnitStats.Armor;
                _unitController.UnitStats.Armor.ClearValue = 0;
                _unitController.UnitStats.Health.ClearValue -= Mathf.Clamp(breakThrowDamage, 0,_unitController.UnitStats.Health);
            }
        }
        else
        {
            int breakThrowDamage = result.Damage - _unitController.UnitStats.Armor;
            _unitController.UnitStats.Armor.ClearValue -= Mathf.Clamp(result.Damage, 0, _unitController.UnitStats.Armor);
            _unitController.UnitStats.Health.ClearValue -= Mathf.Clamp(breakThrowDamage, 0,_unitController.UnitStats.Health);
        }
    }
}
