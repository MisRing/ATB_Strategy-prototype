using System;
using UnityEngine;
using System.Collections.Generic;

public class UnitAbilityController : MonoBehaviour
{
    [SerializeField] private AbilityBasic[] _abilities;
    private AbilityBasic _currentAbility;

    [HideInInspector] public UnitController Unit;

    public void Init(UnitController unit)
    {
        Unit = unit;
        TurnManager.EnterWaitingQ(Unit);

        foreach (var ability in _abilities)
        {
            ability.Init(this);
        }
    }

    public void SelectAbility(int index, AbilityData data)
    {
        if (index >= _abilities.Length || index < 0) return;
        if (_currentAbility == _abilities[index]) return;

        DeselectAbility();

        _currentAbility = _abilities[index];

        _currentAbility.EnterPrepare(data);
    }

    public void DeselectAbility()
    {
        if (_currentAbility != null)
        {
            _currentAbility.ExitPrepare();
            _currentAbility = null;
        }
    }

    public bool ExecuteAbility(AbilityData data)
    {
        _currentAbility.UpdateData(data);
        if (_currentAbility.Execute())
        {
            Unit.State = UnitState.Engaged;
            TurnManager.ExitWaitingQ(Unit);
            return true;
        }

        return false;
    }

    public void FinishExecuteAbility()
    {
        Unit.State = UnitState.WaitingForOrder;
        TurnManager.EnterWaitingQ(Unit);
    }

    public void UpdateAbilityData(AbilityData data)
    {
        _currentAbility.UpdateData(data);
    }
}