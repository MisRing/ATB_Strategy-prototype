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

        if (_onPrepare)
        {
            _skillController.Unit.PreviewAnimator.AimToTarget(
                _unitCombat.Targets[_currentTarget].BodyParts.Body
                );
        }
    }

    public override bool Execute(out int cost)
    {
        cost = _skillCost;

        if (!CanExecute()) return false;
        
        StartCoroutine(Fire(_unitCombat.Targets[_currentTarget]));

        return true;
    }

    private IEnumerator Fire(CombatObject target)
    {
        float duration = TurnManager.TurnTime * _skillCost;
        float aimDuration = duration * 0.2f;
        float shootDuration = duration * 0.45f;
        float endDuration = duration * 0.35f;
        
        Transform targetTransform = target.BodyParts.Body;
        
        yield return _skillController.Unit.Animator.Aim(aimDuration, targetTransform);
        
        bool shoot = target != null && _unitCombat.Targets.Contains(target);

        yield return _skillController.Unit.Animator.PrepareShoot(shootDuration / 3f, targetTransform);
        
        if (shoot)
        {
            HitResult hitResult;
            bool hit = CombatService.CalculateHit(SelectedTargetContext, out hitResult);
            
            float timeToHit = _unitCombat.Weapon.WeaponAnimator.FireAnimation(targetTransform, hit);

            StartCoroutine(DealDamage(target, hitResult, hit, timeToHit));
        }
        else
        {
            GameLogService.ShowMessage("Target out of vision!", _skillController.Unit.Combat.BodyParts.Body, 0f, 0, -1f);
        }
        
        yield return _skillController.Unit.Animator.Shoot(shootDuration / 3f, targetTransform, shoot);
        
        yield return _skillController.Unit.Animator.EndShoot(shootDuration / 3f, targetTransform);
        yield return _skillController.Unit.Animator.EndAim(endDuration, targetTransform);
    }

    private IEnumerator DealDamage(CombatObject target, HitResult hitResult, bool hit, float delay)
    {
        if (hit)
        {
            GameLogService.ShowMessage(
                (hitResult.IsCritical ? "Critical! " : "") + "-" + hitResult.Damage,
                target.BodyParts.Body,
                delay,
                5,
                2f,
                true
                );
        }
        else
        {
            GameLogService.ShowMessage("Miss!", target.BodyParts.Body, delay, 3, 1f, true);
        }
        
        while (delay > 0)
        {
            delay -= TimeService.TimeSpeedDelta;
            yield return null;
        }

        if (hit)
        {
            target.GetDamage(hitResult);
        }
    }
}
