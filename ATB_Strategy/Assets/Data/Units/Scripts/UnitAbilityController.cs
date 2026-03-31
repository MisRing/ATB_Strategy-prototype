using System;
using UnityEngine;
using System.Collections.Generic;

public class UnitAbilityController : MonoBehaviour
{
    public AbilityBasic DefaultAbility;
    public AbilityBasic[] Abilities;
    public AbilityBasic CurrentAbility;

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
    }

    public bool SelectAbility(int index)
    {
        if (index == -1)
        {
            ChangeAbility(DefaultAbility);

            return true;
        }
        if (!Abilities.IsValidIndex(index)) return false;
        
        ChangeAbility(Abilities[index]);

        return true;
    }
    
    private void ChangeAbility(AbilityBasic ability)
    {
        CurrentAbility?.ExitPrepare();
        CurrentAbility = ability;
        CurrentAbility.EnterPrepare();
    }

    public void UpdateAbilityData(AbilityData data)
    {
        CurrentAbility?.UpdateData(data);
    }
    
    public bool ExecuteAbility()
    {
        if (!CurrentAbility) return false;
        
        if (CurrentAbility.Execute())
        {
            CurrentAbility.OnAbilityFinished += AbilityFinished;
            Unit.State = UnitState.Engaged;
            CurrentAbility.ExitPrepare();
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
            ability.OnAbilityFinished += AbilityFinished;
            Unit.State = UnitState.Engaged;
            return true;
        }

        return false;
    }

    private void AbilityFinished(AbilityBasic ability)
    {
        ability.OnAbilityFinished -= AbilityFinished;

        Unit.State = UnitState.WaitingForOrder;
        OnAbilityFinished?.Invoke(Unit);
    }

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