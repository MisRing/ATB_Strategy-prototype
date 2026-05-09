using System;
using UnityEngine;
using System.Collections.Generic;

public class UnitCombat : CombatObject
{
    public WeaponController Weapon;
    
    public override CombatBodyParts BodyParts { get => _bodyParts; }
    [SerializeField] private CombatBodyParts _bodyParts;

    public override UnitOwner Owner { get => _unit.Owner; }
    
    public List<CombatTarget> Targets = new List<CombatTarget>();
    
    public event Action OnUnitDie;

    private UnitController _unit;

    public void Init(UnitController unit)
    {
        _unit = unit;
    }

    public void Start()
    {
        SetVisibility();
    }

    private protected override void OnEnable()
    {
        base.OnEnable();
        CombatManager.OnVisibilityChanged += SetVisibility;
    }

    private protected override void OnDisable()
    {
        base.OnDisable();
        CombatManager.OnVisibilityChanged -= SetVisibility;
    }

    public void SetVisibility()
    {
        Targets = CombatManager.GetCombats(this, _unit.Stats.VisionRange, _unit.Owner);
    }

    public int CheckTarget(UnitController unit)
    {
        for (int i = 0; i < Targets.Count; i++)
        {
            if (Targets[i].Target.gameObject.layer == LayerMask.NameToLayer("Unit"))
            {
                if (Targets[i].Target.gameObject == unit.gameObject)
                {
                    return i;
                }
            }
        }

        return -1;
    }
    
    public override int GetDodge(Vector3 dealerPosition)
    {
        if (_unit.Agent.IsMoving) return _unit.Stats.Dodge;
        
        int dodge = CombatService.CalculateCoverDodge(_unit.Agent.CurrentTile, _unit.Stats.Dodge, dealerPosition);
        
        return dodge;
    }

    public override void GetDamage(HitResult result)
    {
        if (result.IsCritical)
        {
            if (_unit.Stats.Armor >= result.Damage)
            {
                _unit.Stats.Armor.ClearValue = 0;
            }
            else
            {
                int breakThrowDamage = result.Damage - _unit.Stats.Armor;
                _unit.Stats.Armor.ClearValue = 0;
                _unit.Stats.Health.ClearValue -= Mathf.Clamp(breakThrowDamage, 0,_unit.Stats.Health);
            }
        }
        else
        {
            int breakThrowDamage = result.Damage - _unit.Stats.Armor;
            _unit.Stats.Armor.ClearValue -= Mathf.Clamp(result.Damage, 0, _unit.Stats.Armor);
            _unit.Stats.Health.ClearValue -= Mathf.Clamp(breakThrowDamage, 0,_unit.Stats.Health);
        }

        if (_unit.Stats.Health <= 0)
        {
            Die(result);
        }
    }

    public void Die(HitResult lastHit)
    {
        _unit.State = UnitState.Dead;
        _unit.Animator.DeathAnimation(lastHit.Dealer.Position);
        _unit.Agent.InterruptMovement();
        _unit.Agent.RemoveFromGrid();
        TurnManager.RemoveUnitFromBusyQ(_unit);
        
        CombatManager.UnregisterCombat(this);
        CombatManager.TriggerVisibilityReset();
        OnUnitDie?.Invoke();
    }
}
