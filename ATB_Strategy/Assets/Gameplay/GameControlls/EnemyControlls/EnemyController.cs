using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private List<UnitController> _units = new List<UnitController>();
    private UnitAIController[] _unitAIControls;


    public void Init(List<UnitController> units, UnitAIController[] unitAIs)
    {
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
        for (int i = 0; i < _units.Count; i++)
        {
            StartUnitAbility(_units[i]);
        }
    }

    private void StartUnitAbility(UnitController unit)
    {
        if (!_units.Contains(unit)) return;

        int unitIndex = _units.IndexOf(unit);

        UnitAIContext context = _unitAIControls[unitIndex].GetDecision(GetAllCombats());

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

    private List<CombatTarget> GetAllCombats()
    {
        List<CombatTarget> allCombats = new List<CombatTarget>();

        foreach (UnitController unit in _units)
        {
            foreach(CombatTarget target in unit.Combat.Targets)
            {
                if (allCombats.Contains(target)) continue;

                allCombats.Add(target);
            }
        }

        return allCombats;
    }
}
