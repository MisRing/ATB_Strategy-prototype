using System;
using UnityEngine;

public class UnitCombat : CombatAbstract
{
    public override UnitOwner Owner { get => _unitController.Owner; }

    private UnitController _unitController;

    public void Init(UnitController unit)
    {
        _unitController = unit;
    }
}
