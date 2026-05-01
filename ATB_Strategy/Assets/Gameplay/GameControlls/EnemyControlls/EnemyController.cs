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
        int unitIndex = _units.IndexOf(unit);
        if (unitIndex == -1) return;


        UnitAIContext context = _unitAIControls[unitIndex].GetDecision();

        bool success = context.Decision switch
        {
            UnitAIDecision.Attack   => TryAttack(unit, context),
            UnitAIDecision.Relocate => TryRelocate(unit),
            _                       => false
        };
        
        if (!success)
        {
            if (!TryFallback(unit))
            {
                Debug.LogWarning("Fallback Error!");
            }
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
    
    private bool TryRelocate(UnitController unit)
    {
        List<GridTile> tiles = GridParameters.LevelGrid.GetTilesWithCover(unit.transform.position, 10f);

        tiles.Shuffle(); // better choice

        foreach (var tile in tiles)
        {
            if (tile.Owner != null) continue;

            Vector3 pos = GridParameters.LevelGrid.GetTileWorldPos(tile);

            PointData data = new PointData
            {
                Position = pos
            };

            if (TryExecute(unit, 0, data)) return true;
        }

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
}
