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

    public void ExecuteAbility(AbilityData data)
    {
        _currentAbility.UpdateData(data);
        _currentAbility.Execute();
    }
}