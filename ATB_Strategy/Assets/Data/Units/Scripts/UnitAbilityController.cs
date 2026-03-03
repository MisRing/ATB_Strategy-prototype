using System;
using UnityEngine;
using System.Collections.Generic;

public class UnitAbilityController : MonoBehaviour
{
    [SerializeField] private AbilityBasic[] _abilities;
    private AbilityBasic _currentAbility;

    [HideInInspector] public UnitComponent Unit;

    public void Init(UnitComponent unit)
    {
        Unit = unit;

        foreach(var ability in _abilities)
        {
            ability.Init(this);
        }
    }

    public void SelectAbility(int index, AbilityData data)
    {
        if (index >= _abilities.Length) return;
        if (_currentAbility == _abilities[index]) return;
        if (_currentAbility && _currentAbility.Status == AbilityStatus.Executing) return;

        DeselectAbility();

        _currentAbility = _abilities[index];

        _currentAbility.EnterPrepare(data);
    }

    public void DeselectAbility()
    {
        if (_currentAbility != null && _currentAbility.Status != AbilityStatus.Executing)
        {
            _currentAbility.ExitPrepare();
            _currentAbility = null;
        }
    }

    public bool ExecuteAbility(AbilityData data)
    {
        if (_currentAbility.Status != AbilityStatus.Executing)
        {
            _currentAbility.UpdateData(data);
            if (_currentAbility.Execute())
            {
                Unit.State = UnitState.Engaged;
                return true;
            }
        }

        return false;
    }

    public void FinishExecuteAbility()
    {
        Unit.State = UnitState.WaitingForOrder;
    }

    public void UpdateAbilityData(AbilityData data)
    {
        if (_currentAbility && _currentAbility.Status != AbilityStatus.Executing)
        {
            _currentAbility.UpdateData(data);
        }
    }
}