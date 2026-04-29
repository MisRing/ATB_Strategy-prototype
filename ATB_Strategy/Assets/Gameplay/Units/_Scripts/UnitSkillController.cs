using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UnitSkillController : MonoBehaviour
{
    [Header("Skills")]
    public BasicSkill DefaultMovement;
    public BasicSkill DefaultAttack;
    [SerializeField] private BasicSkill[] _skills;
    public int SkillsCount
    {
        get
        {
            int count = 0;
            if(DefaultMovement) count++;
            if(DefaultAttack) count++;
            count += _skills.Length;
            return count;
        }
    }
    public BasicSkill CurrentSkill;

    [HideInInspector] public UnitController Unit;

    public event Action<UnitController> OnSkillFinished;

    public void Init(UnitController unit)
    {
        Unit = unit;

        DefaultMovement?.Init(this);
        DefaultAttack?.Init(this);
        foreach (BasicSkill skill in _skills)
        {
            skill.Init(this);
        }
    }

    public bool SelectSkill(int index)
    {
        BasicSkill skill = GetSkillByIndex(index);

        if (!skill) return false;
        
        return ChangeSkill(skill);
    }
    
    private bool ChangeSkill(BasicSkill skill)
    {
        if (skill == CurrentSkill && skill.OnPrepare) return true;
        if (skill.IsOnCooldown()) return false;
        CurrentSkill?.ExitPrepare();
        CurrentSkill = skill;
        CurrentSkill.EnterPrepare();
        return true;
    }

    public void UpdateAbilityData(SkillData data)
    {
        CurrentSkill?.UpdateData(data);
    }
    
    public bool ExecuteSkill(out bool isInstant)
    {
        isInstant = false;
        if (!CurrentSkill) return false;
        int cost = 0;
        if (CurrentSkill.Execute(ref cost))
        {
            if (cost > 0)
            {
                Unit.State = UnitState.Engaged;
                TurnManager.EnterBusyQ(Unit, cost);
            }
            else
            {
                isInstant = true;
            }
            CurrentSkill.ExitPrepare();
            return true;
        }

        return false;
    }
    
    public bool ForceExecuteSkill(int index, SkillData data, out bool isInstant)
    {
        isInstant = false;

        BasicSkill skill = GetSkillByIndex(index);

        skill.UpdateData(data);
        int cost = 0;
        if (skill.Execute(ref cost))
        {
            if (cost > 0)
            {
                Unit.State = UnitState.Engaged;
                TurnManager.EnterBusyQ(Unit, cost);
            }
            else
            {
                isInstant = true;
            }
            return true;
        }

        return false;
    }

    public BasicSkill GetSkillByIndex(int index)
    {
        // TODO: BETTER SELECTION
        if (!DefaultMovement)
        {
            index++;
        }
        if (!DefaultAttack && index >= 1)
        {
            index++;
        }
        switch (index)
        {
            case (0):
                return DefaultMovement;
            case (1):
                return DefaultAttack;
            default:
                index -= 2;
                break;
        }
        if (!_skills.IsValidIndex(index)) return null;
        return _skills[index];
    }

    public void FinishSkill()
    {
        Unit.State = UnitState.WaitingForOrder;
        Unit.Combat.SetVisibility();
        OnSkillFinished?.Invoke(Unit);
    }
}