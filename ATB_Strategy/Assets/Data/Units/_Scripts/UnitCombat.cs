using System;
using UnityEngine;

public class UnitCombat : CombatAbstract
{
    private UnitController _unitController;

    public void Init(UnitController unit)
    {
        _unitController = unit;
    }
}
