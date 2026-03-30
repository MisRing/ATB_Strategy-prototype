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

        DefaultAbility.Init(this);
        foreach (var ability in Abilities)
        {
            ability.Init(this);
        }
        ChangeAbility(DefaultAbility);
    }
    
    public void SelectDefaultAbility() => ChangeAbility(DefaultAbility);

    public AbilitySelectResult SelectAbility(int index)
    {
        if (!Abilities.IsValidIndex(index)) return AbilitySelectResult.Invalid;

        if (_currentAbility == Abilities[index]) return AbilitySelectResult.Same;
        
        ChangeAbility(Abilities[index]);

        return AbilitySelectResult.Changed;
    }

    public bool ExecuteAbility()
    {
        if (!_currentAbility) return false;
        
        if (_currentAbility.Execute())
        {
            Unit.State = UnitState.Engaged;
            _currentAbility.ExitPrepare();
            return true;
        }

        return false;
    }
    
    public bool ForceExecuteAbility(int index, AbilityData data)
    {
        if (!Abilities.IsValidIndex(index) && index != -1) return false;
        
        AbilityBasic ability = index == -1 ? DefaultAbility : Abilities[index];
        
        ability.UpdateData(data);

        if (ability.Execute())
        {
            Unit.State = UnitState.Engaged;
            return true;
        }

        return false;
    }

    public void UpdateAbilityData(AbilityData data)
    {
        _currentAbility?.UpdateData(data);
    }
    
    private void ChangeAbility(AbilityBasic ability)
    {
        _currentAbility?.ExitPrepare();
        _currentAbility = ability;
        _currentAbility.EnterPrepare();
    }
    
    public void FinishExecuteAbility()
    {
        Unit.State = UnitState.WaitingForOrder;
        OnAbilityFinished?.Invoke(Unit);
    }

}

public enum AbilitySelectResult
{
    Changed,
    Same,
    Invalid
}

public static class CollectionExtensions
{
    public static bool IsValidIndex<T>(this IList<T> collection, int index)
    {
        return index >= 0 && index < collection.Count;
    }
    
    public static bool IsValidIndex<T>(this T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }
}