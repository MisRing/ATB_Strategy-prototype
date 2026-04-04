using System;
using UnityEngine;
using System.Collections.Generic;

public class UnitSkillController : MonoBehaviour
{
    public BasicSkill DefaultSkill;
    public BasicSkill[] Skills;
    public BasicSkill CurrentSkill;

    [HideInInspector] public UnitController Unit;

    public event Action<UnitController> OnSkillFinished;

    public void Init(UnitController unit)
    {
        Unit = unit;

        DefaultSkill?.Init(this);
        foreach (var skill in Skills)
        {
            skill.Init(this);
        }
    }

    public bool SelectSkill(int index)
    {
        if (index == -1)
        {
            ChangeSkill(DefaultSkill);

            return true;
        }
        if (!Skills.IsValidIndex(index)) return false;
        
        ChangeSkill(Skills[index]);

        return true;
    }
    
    private void ChangeSkill(BasicSkill skill)
    {
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
        
        if (CurrentSkill.Execute())
        {
            Unit.State = UnitState.Engaged;
            CurrentSkill.ExitPrepare();
            return true;
        }

        return false;
    }
    
    public bool ForceExecuteSkill(int index, SkillData data)
    {
        if (!Skills.IsValidIndex(index) && index != -1) return false;
        
        BasicSkill skill = index == -1 ? DefaultSkill : Skills[index];
        
        skill.UpdateData(data);

        if (skill.Execute())
        {
            Unit.State = UnitState.Engaged;
            return true;
        }

        return false;
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