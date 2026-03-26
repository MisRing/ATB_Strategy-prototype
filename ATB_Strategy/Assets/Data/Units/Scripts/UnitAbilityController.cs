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

        DefaultAbility.Init(this);
        foreach (var ability in Abilities)
        {
            ability.Init(this);
        }
        _currentAbility = DefaultAbility;
        _currentAbility.EnterPrepare();
    }

    public void SelectAbility(int index) //
    {
        if (index == -1)
        {
            _currentAbility.ExitPrepare();
            _currentAbility = DefaultAbility;
            _currentAbility.EnterPrepare();
            return;
        }
        
        if (index >= Abilities.Length || index < 0) return;
        if (_currentAbility == Abilities[index])
        {
            ExecuteAbility();

            return;
        }

        _currentAbility.ExitPrepare();

        _currentAbility = Abilities[index];

        _currentAbility.EnterPrepare();
    }

    public bool ExecuteAbility()
    {
        if (_currentAbility == null) return false;
        
        if (_currentAbility.Execute())
        {
            Unit.State = UnitState.Engaged;
            _currentAbility.ExitPrepare();
            return true;
        }

        return false;
    }
    
    public bool ForceExecuteAbility(AbilityData data)
    {
        if (_currentAbility == null) return false;
        
        _currentAbility.UpdateData(data);

        return ExecuteAbility();
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