using System;
using UnityEngine;
using System.Collections.Generic;

public class UnitSkillController : MonoBehaviour
{
    public BasicSkill DefaultMovementSkill;
    public BasicSkill DefaultAttackSkill;
    //public BasicSkill DefaultSkill;
    [SerializeField] private BasicSkill[] skills;
    public int SkillsCount
    {
        get
        {
            int count = 0;
            if(DefaultMovementSkill) count++;
            if(DefaultAttackSkill) count++;
            count += skills.Length;
            return count;
        }
    }
    public BasicSkill CurrentSkill;

    [HideInInspector] public UnitController Unit;

    public event Action<UnitController> OnSkillFinished;

    public void Init(UnitController unit)
    {
        Unit = unit;

        DefaultMovementSkill?.Init(this);
        DefaultAttackSkill?.Init(this);
        foreach (var skill in skills)
        {
            skill.Init(this);
        }
    }

    public bool SelectSkill(int index)
    {
        BasicSkill skill = GetSkillByIndex(index);

        if (skill == null) return false;
        
        ChangeSkill(skill);

        return true;
    }
    
    private void ChangeSkill(BasicSkill skill)
    {
        if (skill == CurrentSkill && skill.OnPrepare) return;
        CurrentSkill?.ExitPrepare();
        CurrentSkill = skill;
        CurrentSkill.EnterPrepare();
    }

    public void UpdateAbilityData(SkillData data)
    {
        CurrentSkill?.UpdateData(data);
    }
    
    public bool ExecuteSkill()
    {
        if (!CurrentSkill) return false;
        int cost = 0;
        if (CurrentSkill.Execute(ref cost))
        {
            Unit.State = UnitState.Engaged;
            TurnManager.EnterBusyQ(Unit, cost);
            CurrentSkill.ExitPrepare();
            return true;
        }

        return false;
    }
    
    public bool ForceExecuteSkill(int index, SkillData data)
    {
        BasicSkill skill = GetSkillByIndex(index);

        skill.UpdateData(data);
        int cost = 0;
        if (skill.Execute(ref cost))
        {
            Unit.State = UnitState.Engaged;
            TurnManager.EnterBusyQ(Unit, cost);
            return true;
        }

        return false;
    }

    public BasicSkill GetSkillByIndex(int index)
    {
        // TODO: BETTER SELECTION
        if (!DefaultMovementSkill)
        {
            index++;
        }
        if (!DefaultAttackSkill && index >= 1)
        {
            index++;
        }
        switch (index)
        {
            case (0):
                return DefaultMovementSkill;
            case (1):
                return DefaultAttackSkill;
            default:
                index -= 2;
                break;
        }
        if (!skills.IsValidIndex(index)) return null;
        return skills[index];
    }

    public void FinishSkill()
    {
        Unit.State = UnitState.WaitingForOrder;
        OnSkillFinished?.Invoke(Unit);
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