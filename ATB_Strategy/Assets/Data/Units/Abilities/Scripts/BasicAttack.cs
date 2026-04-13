using System;
using UnityEngine;
using System.Collections.Generic;

public class BasicAttack : BasicSkill, ITargetSwitchable
{
    public GameObject CurrentTarget
    {
        get
        {
            if (_targets == null || _targets.Count == 0) return null;

            return (_targets[_currentTarget] as CombatAbstract).gameObject;
        }
    }
    
    public event Action<GameObject> OnTargetSwitched;

    private int _currentTarget = 0;
    private List<ICombat> _targets;
    
    public override void Init(UnitSkillController skillController)
    {
        base.Init(skillController);
        SkillName = "Basic attack";
        RequiredDataType = typeof(TargetData);
    }

    public override void EnterPrepare()
    {
        _targets = CombatService.GetCombats(transform.position, _skillController.Unit.UnitStats.Vision, _skillController.Unit.Owner);
        
        if (_targets.Count != 0)
        {
            OnTargetSwitched?.Invoke((_targets[_currentTarget] as CombatAbstract).gameObject);
        }

        base.EnterPrepare();
    }

    public override void ExitPrepare()
    {
        _targets.Clear();
        _currentTarget = 0;
        base.ExitPrepare();
    }

    public override void UpdateData(SkillData skillData)
    {
        if (_skillController.Unit.State != UnitState.WaitingForOrder) return;

        base.UpdateData(skillData);
    }
    
    public void Switch()
    {
        if(_targets.Count == 0) return;
        
        _currentTarget = (_currentTarget + 1) % _targets.Count;
        
        OnTargetSwitched?.Invoke((_targets[_currentTarget] as CombatAbstract).gameObject);
    }

    public override bool Execute(ref int cost)
    {
        if(_targets.Count == 0) return false;

        cost = 8;

        Debug.Log("Start executing <" + SkillName + "> | Cost: " + cost);

        _skillController.Unit.UnitAnimator.AnimateAim(_targets[_currentTarget].Position, TurnManager.TurnTime * cost);

        return true;
    }

}
