using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class BasicAttack : BasicSkill, ITargetSwitchable
{
    //----------------------------ITargetSwitchable----------------------------

    public int TargetIndex { get { return _currentTarget; } }

    public int TargetsCount { get { return _unitCombat.Targets.Count; } }

    public CombatContext SelectedTargetContext { get; set; }

    private int _currentTarget = 0;

    //----------------------------OTHER-----------------------------------------

    private UnitCombat _unitCombat;
    
    public override void Init(UnitSkillController skillController)
    {
        base.Init(skillController);
        SkillName = "Basic attack";
        RequiredDataType = typeof(TargetData);
        _unitCombat = skillController.Unit.Combat;
        _skillCost = 4;
        _skillCooldown = 0;
    }

    public override void EnterPrepare()
    {
        base.EnterPrepare();
    }

    public override void ExitPrepare()
    {
        _currentTarget = 0;
        _skillController.Unit.PreviewAnimator.EndPreview();
        base.ExitPrepare();
    }

    public override void UpdateData(SkillData skillData)
    {
        if (_skillController.Unit.State != UnitState.WaitingForOrder) return;

        base.UpdateData(skillData);
    }

    public override bool CanExecute()
    {
        if (_skillCooldownTimer > 0) return false;
        return _unitCombat.Targets.Count > 0;
    }

    public void Switch(int index)
    {
        if(_unitCombat.Targets.Count == 0) return;
        if (!_unitCombat.Targets.IsValidIndex(index)) return;
        
        _currentTarget = index;

        SelectedTargetContext = CombatService.CalculateHitContext(
            _skillController.Unit.Combat,
            _skillController.Unit.Stats.Accuracy,
            _skillController.Unit.Combat.Weapon,
            _unitCombat.Targets[_currentTarget]
            );
        
        _skillController.Unit.PreviewAnimator.AimToTarget(_unitCombat.Targets[_currentTarget].Target.BodyParts.Body.Transform);
    }

    public override bool Execute(ref int cost)
    {
        if (!CanExecute()) return false;

        cost = _skillCost;
        
        StartCoroutine(Fire(_unitCombat.Targets[_currentTarget].Target));

        return true;
    }

    private IEnumerator Fire(CombatObject target)
    {
        float duration = TurnManager.TurnTime * _skillCost;
        float aimDuration = duration * 0.2f;
        float shootDuration = duration * 0.45f;
        float endDuration = duration * 0.35f;
        
        Transform targetTransform = target.BodyParts.Body.Transform;
        
        yield return _skillController.Unit.Animator.Aim(aimDuration, targetTransform);
        
        float visionPercent = CombatService.GetVisionPercent(
            _skillController.Unit.Combat.Position + Vector3.up,
            target
            );
        bool shoot = target != null && visionPercent > 0f;

        if (shoot)
        {
            HitResult hit;
            if (CombatService.CalculateHit(SelectedTargetContext, out hit))
            {
                target.GetDamage(hit);
                GameLogService.ShowMessage((hit.IsCritical ? "Critical! " : "") + "-" + hit.Damage, target.BodyParts.Body.Transform);
            }
            else
            {
                GameLogService.ShowMessage("Miss!", target.BodyParts.Body.Transform);
            }
        }
        else
        {
            GameLogService.ShowMessage("Target out of vision!", _skillController.Unit.Combat.BodyParts.Body.Transform);
        }
        
        yield return _skillController.Unit.Animator.Shoot(shootDuration, targetTransform, shoot);
        yield return _skillController.Unit.Animator.EndAim(endDuration, targetTransform);
    }

}
