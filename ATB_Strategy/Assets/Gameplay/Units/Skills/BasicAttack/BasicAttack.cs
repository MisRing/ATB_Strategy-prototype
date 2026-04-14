using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class BasicAttack : BasicSkill, ITargetSwitchable
{
    [SerializeField] private int _cost = 8;
    
    public GameObject CurrentTarget
    {
        get
        {
            if (_targets == null || _targets.Count == 0) return null;

            return (_targets[_currentTarget] as CombatAbstract).gameObject;
        }
    }
    public int TargetIndex { get { return _currentTarget; } }

    public int TargetsCount { get { return _targets.Count; } }
    
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
        
        _skillController.Unit.UnitPreviewAnimator.AimToTarget(_targets[_currentTarget].BodyParts[0].transform);

        base.EnterPrepare();
    }

    public override void ExitPrepare()
    {
        _targets.Clear();
        _currentTarget = 0;
        _skillController.Unit.UnitPreviewAnimator.EndPreview();
        base.ExitPrepare();
    }

    public override void UpdateData(SkillData skillData)
    {
        if (_skillController.Unit.State != UnitState.WaitingForOrder) return;

        base.UpdateData(skillData);
    }
    
    public void Switch(int index)
    {
        if(_targets.Count == 0) return;
        if (!_targets.IsValidIndex(index)) return;
        
        _currentTarget = index;
        
        _skillController.Unit.UnitPreviewAnimator.AimToTarget(_targets[_currentTarget].BodyParts[0].transform);
        
        OnTargetSwitched?.Invoke((_targets[_currentTarget] as CombatAbstract).gameObject);
    }

    public override bool Execute(ref int cost)
    {
        if(_targets.Count == 0) return false;

        cost = _cost;
        
        StartCoroutine(Fire(_targets[_currentTarget] as CombatAbstract));

        return true;
    }

    private IEnumerator Fire(CombatAbstract target)
    {
        float duration = TurnManager.TurnTime * _cost;
        float aimDuration = duration * 0.2f;
        float shootDuration = duration * 0.45f;
        float endDuration = duration * 0.35f;
        
        Transform targetTransform = target.BodyParts[0].transform;
        
        yield return _skillController.Unit.UnitAnimator.Aim(aimDuration, targetTransform);
        bool shoot = target != null;
        yield return _skillController.Unit.UnitAnimator.Shoot(shootDuration, targetTransform, shoot);
        yield return _skillController.Unit.UnitAnimator.EndAim(endDuration, targetTransform);
    }

}
