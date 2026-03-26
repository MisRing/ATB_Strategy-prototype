using System;
using UnityEngine;
using System.Collections.Generic;

public class UnitAbilityController : MonoBehaviour
{
    public AbilityBasic DefaultAbility;
    public AbilityBasic[] Abilities;
    private AbilityBasic _currentAbility;

    [HideInInspector] public UnitController Unit;

    public event Action<UnitController> OnAbilityFinished;

    public void Init(UnitController unit)
    {
        Unit = unit;
        TurnManager.EnterWaitingQ(Unit);

        foreach (var ability in Abilities)
        {
            ability.Init(this);
        }
    }

    public void SelectAbility(int index, AbilityData data) //
    {
        if (index >= Abilities.Length || index < 0) return;
        if (_currentAbility == Abilities[index]) return;

        DeselectAbility();

        _currentAbility = Abilities[index];

        _currentAbility.EnterPrepare(data);
    }

    public void DeselectAbility() //
    {
        if (_currentAbility != null)
        {
            _currentAbility.ExitPrepare();
            _currentAbility = null;
        }
    }

    public bool ExecuteAbility(AbilityData data)
    {
        if (_currentAbility == null) return false;
        
        _currentAbility.UpdateData(data);
        if (_currentAbility.Execute())
        {
            Unit.State = UnitState.Engaged;
            return true;
        }

        return false;
    }

    public void FinishExecuteAbility()
    {
        Unit.State = UnitState.WaitingForOrder;
        OnAbilityFinished?.Invoke(Unit);
    }

    public void UpdateAbilityData(AbilityData data)
    {
        if (_currentAbility == null) return;
        _currentAbility.UpdateData(data);
    }
}