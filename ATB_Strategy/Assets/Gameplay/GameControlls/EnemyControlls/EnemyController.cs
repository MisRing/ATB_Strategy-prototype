using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private UnitOwner _teamOwner;
    private List<UnitController> _units = new List<UnitController>();
    private UnitAIController[] _unitAIControls;

    private Dictionary<CombatTarget, AITarget> _allTargets;

    public void Init(UnitOwner owner, List<UnitController> units, UnitAIController[] unitAIs)
    {
        _teamOwner = owner;
        _units = units;
        _unitAIControls = unitAIs;
    }

    private void OnEnable()
    {
        for (int i = 0; i < _units.Count; i++)
        {
            if(!_units[i]) continue;
            _units[i].SkillController.OnSkillFinished += StartUnitAbility;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _units.Count; i++)
        {
            if(!_units[i]) continue;
            _units[i].SkillController.OnSkillFinished -= StartUnitAbility;
        }
    }

    private void Start()
    {
        SetTargets();
        for (int i = 0; i < _units.Count; i++)
        {
            StartUnitAbility(_units[i]);
        }
    }

    private void StartUnitAbility(UnitController unit)
    {
        if (!_units.Contains(unit)) return;

        UpdateTargets();

        int unitIndex = _units.IndexOf(unit);

        UnitAIContext context = _unitAIControls[unitIndex].GetDecision(_allTargets.Values.ToList());

        bool success = context.Decision switch
        {
            UnitAIDecision.Attack   => TryAttack(unit, context),
            UnitAIDecision.Relocate => TryRelocate(unit, context),
            _                       => false
        };
        
        if (!success)
        {
            TryFallback(unit);
            Debug.LogWarning("Fallback Error! < " + unit.name + " | Team: " + unit.Owner + " | Decision: " + context.Decision + " >");
        }
    }

    private bool TryAttack(UnitController unit, UnitAIContext context)
    {
        BasicSkill skill = unit.SkillController.GetSkillByIndex(1);

        if (skill is not ITargetSwitchable attackSkill) return false;

        if (context.AttackTargetID < 0) return false;

        attackSkill.Switch(context.AttackTargetID);

        return TryExecute(unit, 1, null);
    }

    private bool TryRelocate(UnitController unit, UnitAIContext context)
    {

        PointData data = new PointData
        {
            Position = context.TargetPosition
        };

        if (TryExecute(unit, 0, data)) return true;

        return false;
    }
    
    private bool TryExecute(UnitController unit, int skillIndex, SkillData data)
    {
        return unit.SkillController.ForceExecuteSkill(skillIndex, data, out bool isInstant);
    }
    
    private bool TryFallback(UnitController unit)
    {
        return TryExecute(unit, 3, null);
    }

    private void SetTargets()
    {
        List<CombatTarget> combats = CombatManager.GetAllCombats(_teamOwner);

        _allTargets = new Dictionary<CombatTarget, AITarget>();
        foreach (CombatTarget combat in combats)
        {
            _allTargets.Add(combat, new AITarget(combat.Target.Position));
        }
    }

    private void UpdateTargets()
    {
        foreach (AITarget target in _allTargets.Values)
        {
            target.IsVisible = false;
        }

        List<CombatTarget> visibleCombats = GetAllVisibleCombats();

        foreach(CombatTarget visibleTarget in visibleCombats)
        {
            if (!_allTargets.ContainsKey(visibleTarget)) continue;

            _allTargets[visibleTarget].IsVisible = true;
            _allTargets[visibleTarget].Position = visibleTarget.Target.Position;
        }    
    }

    private List<CombatTarget> GetAllVisibleCombats()
    {
        List<CombatTarget> allCombats = new List<CombatTarget>();

        foreach (UnitController unit in _units)
        {
            if (unit.State == UnitState.Dead) continue;

            foreach(CombatTarget target in unit.Combat.Targets)
            {
                if (allCombats.Contains(target)) continue;

                allCombats.Add(target);
            }
        }

        return allCombats;
    }
}

public class AITarget
{
    public bool IsVisible = false;
    public Vector3 Position;
    public int Priority = 5;

    public AITarget(Vector3 position)
    {
        Position = position;
    }
}
