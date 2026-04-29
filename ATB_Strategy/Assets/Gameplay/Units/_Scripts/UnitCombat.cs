using System;
using UnityEngine;
using System.Collections.Generic;

public class UnitCombat : CombatObject
{
    public WeaponController Weapon;
    
    public override CombatBodyParts BodyParts { get => _bodyParts; }
    [SerializeField] private CombatBodyParts _bodyParts;

    public override UnitOwner Owner { get => _unitController.Owner; }
    
    public List<CombatTarget> Targets = new List<CombatTarget>();
    public event Action OnUnitDie;

    private UnitController _unitController;

    public void Init(UnitController unit)
    {
        _unitController = unit;
    }

    public void Start()
    {
        SetVisibility();
    }

    private protected override void OnEnable()
    {
        base.OnEnable();
        CombatService.OnVisibilityChanged += SetVisibility;
    }

    private protected override void OnDisable()
    {
        base.OnDisable();
        CombatService.OnVisibilityChanged -= SetVisibility;
    }

    public override int GetDodge(Vector3 dealerPosition)
    {
        if (_unitController.Agent.IsMoving) return _unitController.Stats.Dodge;
        
        Vector3Int tilePos = _unitController.Agent.CurrentTile;
        GridTile tile = GridParameters.LevelGrid.GetTile(tilePos.x, tilePos.z, tilePos.y);
        int dodge = CombatService.CalculateCoverDodge(tile, _unitController.Stats.Dodge, dealerPosition);
        
        return dodge;
    }

    public void SetVisibility()
    {
        Targets = CombatService.GetCombats(this, _unitController.Stats.VisionRange, _unitController.Owner);
    }

    public int CheckTarget(UnitController unit)
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            if (Targets[i].Target.gameObject.layer == LayerMask.NameToLayer("Unit"))
            {
                if (Targets[i].Target.gameObject.GetComponent<UnitController>() == unit)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public override void GetDamage(HitResult result)
    {
        if (result.IsCritical)
        {
            if (_unitController.Stats.Armor >= result.Damage)
            {
                _unitController.Stats.Armor.ClearValue = 0;
            }
            else
            {
                int breakThrowDamage = result.Damage - _unitController.Stats.Armor;
                _unitController.Stats.Armor.ClearValue = 0;
                _unitController.Stats.Health.ClearValue -= Mathf.Clamp(breakThrowDamage, 0,_unitController.Stats.Health);
            }
        }
        else
        {
            int breakThrowDamage = result.Damage - _unitController.Stats.Armor;
            _unitController.Stats.Armor.ClearValue -= Mathf.Clamp(result.Damage, 0, _unitController.Stats.Armor);
            _unitController.Stats.Health.ClearValue -= Mathf.Clamp(breakThrowDamage, 0,_unitController.Stats.Health);
        }

        if (_unitController.Stats.Health <= 0)
        {
            Die(result);
        }
    }

    public void Die(HitResult lastHit)
    {
        _unitController.State = UnitState.Dead;
        _unitController.Animator.DeathAnimation(lastHit.Dealer.Position);
        _unitController.Agent.RemoveFromGrid();
        
        CombatService.UnregisterCombat(this);
        CombatService.TriggerVisibilityReset();
        OnUnitDie?.Invoke();
    }
}
