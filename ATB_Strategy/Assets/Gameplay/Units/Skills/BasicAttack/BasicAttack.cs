using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class BasicAttack : BasicSkill, ITargetSwitchable
{
    [SerializeField] private int _cost = 8;

    public int TargetIndex { get { return _currentTarget; } }

    public int TargetsCount { get { return _unitCombat.Targets.Count; } }

    public CombatContext CurrentTarget { get; set; }

    public event Action<CombatContext> OnTargetSwitched;

    private int _currentTarget = 0;
    
    private UnitCombat _unitCombat;
    
    public override void Init(UnitSkillController skillController)
    {
        base.Init(skillController);
        SkillName = "Basic attack";
        RequiredDataType = typeof(TargetData);
        _unitCombat = skillController.Unit.UnitCombat;
    }

    public override void EnterPrepare()
    {
        _unitCombat.Targets = CombatService.GetCombats(_unitCombat, _skillController.Unit.UnitStats.VisionRange, _skillController.Unit.Owner);

        Switch(0);
        
        base.EnterPrepare();
    }

    public override void ExitPrepare()
    {
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
        if(_unitCombat.Targets.Count == 0) return;
        if (!_unitCombat.Targets.IsValidIndex(index)) return;
        
        _currentTarget = index;

        CurrentTarget = CombatService.CalculateHitContext(
            _skillController.Unit.UnitCombat,
            _skillController.Unit.UnitStats.Accuracy,
            _skillController.Unit.UnitCombat.Weapon,
            _unitCombat.Targets[_currentTarget]
            );
        
        _skillController.Unit.UnitPreviewAnimator.AimToTarget(_unitCombat.Targets[_currentTarget].Target.BodyParts.Body.Transform);
        
        OnTargetSwitched?.Invoke(CurrentTarget);
    }

    public override bool Execute(ref int cost)
    {
        if(_unitCombat.Targets.Count == 0) return false;

        cost = _cost;
        
        StartCoroutine(Fire(_unitCombat.Targets[_currentTarget].Target));

        return true;
    }

    private IEnumerator Fire(CombatObject target)
    {
        float duration = TurnManager.TurnTime * _cost;
        float aimDuration = duration * 0.2f;
        float shootDuration = duration * 0.45f;
        float endDuration = duration * 0.35f;
        
        Transform targetTransform = target.BodyParts.Body.Transform;
        
        yield return _skillController.Unit.UnitAnimator.Aim(aimDuration, targetTransform);
        
        float visionPercent = CombatService.GetVisionPercent(
            _skillController.Unit.UnitCombat.Position + Vector3.up,
            target
            );
        bool shoot = target != null && visionPercent > 0f;

        if (shoot)
        {
            HitResult hit;
            if (CombatService.CalculateHit(CurrentTarget, out hit))
            {
                target.GetDamage(hit);
                Debug.Log("His success " + hit.Damage);
            }
            else
            {
                Debug.Log("Miss!");
            }
        }
        
        yield return _skillController.Unit.UnitAnimator.Shoot(shootDuration, targetTransform, shoot);
        yield return _skillController.Unit.UnitAnimator.EndAim(endDuration, targetTransform);
    }

}
