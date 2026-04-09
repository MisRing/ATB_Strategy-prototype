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

    private int _currentTarget;
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

        _currentTarget = 0;

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
        
        Debug.Log("Start executing <" + SkillName + "> | Cost: " + 3);

        cost = 3;

        return true;
    }

}
