using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UnitSkillController : MonoBehaviour
{
    [Header("Skills")]
    [SerializeField] private BasicSkill _defaultMovement;
    [SerializeField] private BasicSkill _defaultAttack;
    [SerializeField] private BasicSkill[] _skills;
    
    public BasicSkill CurrentSkill { get { return _currentSkill; } }
    private BasicSkill _currentSkill;
    
    public int SkillsCount
    {
        get
        {
            int count = 0;
            if(_defaultMovement) count++;
            if(_defaultAttack) count++;
            count += _skills.Length;
            return count;
        }
    }

    [HideInInspector] public UnitController Unit;

    public event Action<UnitController> OnSkillFinished;

    public void Init(UnitController unit)
    {
        Unit = unit;

        _defaultMovement?.Init(this);
        _defaultAttack?.Init(this);
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
        if (skill == _currentSkill && skill.OnPrepare) return true;
        if (skill.IsOnCooldown()) return false;
        
        _currentSkill?.ExitPrepare();
        _currentSkill = skill;
        _currentSkill.EnterPrepare();
        return true;
    }

    public void UpdateAbilityData(SkillData data)
    {
        _currentSkill?.UpdateData(data);
    }
    
    public bool ExecuteSkill(out bool isInstant)
    {
        isInstant = false;
        if (!_currentSkill) return false;
        
        int cost = 0;
        if (_currentSkill.Execute(ref cost))
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
            _currentSkill.ExitPrepare();
            return true;
        }

        return false;
    }
    
    public bool ForceExecuteSkill(int index, SkillData data, out bool isInstant)
    {
        isInstant = false;

        BasicSkill skill = GetSkillByIndex(index);
        
        if (!skill) return false;

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
        if (_defaultMovement)
        {
            if (index == 0) return _defaultMovement;
            index--;
        }

        if (_defaultAttack)
        {
            if (index == 0) return _defaultAttack;
            index--;
        }

        if (_skills.IsValidIndex(index))
            return _skills[index];

        return null;
    }

    public void FinishSkill()
    {
        Unit.State = UnitState.WaitingForOrder;
        Unit.Combat.SetVisibility();
        OnSkillFinished?.Invoke(Unit);
    }
}