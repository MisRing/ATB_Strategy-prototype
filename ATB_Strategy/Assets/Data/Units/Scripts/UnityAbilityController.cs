using System;
using UnityEngine;
using System.Collections.Generic;

public class UnityAbilityController : MonoBehaviour
{
    private IAbility[] _abilities;
    private IAbility _currentAbility;

    [HideInInspector] public UnitComponent Unit;

    public void Init(UnitComponent unit)
    {
        Unit = unit;

        _abilities = new IAbility[2];
        _abilities[0] = new MovementAbility();
        _abilities[1] = new EmptyAbility();

        foreach(var ability in _abilities)
        {
            ability.Init(this);
        }
    }

    public void SelectAbility(int index)
    {
        if (index >= _abilities.Length) return;
        if (_currentAbility == _abilities[index]) return;

        if(_currentAbility != null)
        {
            _currentAbility.Cancel();
        }

        _currentAbility = _abilities[index];

        _currentAbility.EnterPrepare();
    }

    public void DeselectAbility()
    {
        if (_currentAbility != null)
        {
            _currentAbility.Cancel();
            _currentAbility = null;
        }
    }
}