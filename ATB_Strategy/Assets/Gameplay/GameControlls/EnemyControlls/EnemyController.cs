using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

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
        
        bool isInstant;

        if (context.Decision == UnitAIDecision.None)
        {
            if (unit.SkillController.ForceExecuteSkill(3, null, out isInstant))
            {
                return;
            }
        }
        else if (context.Decision == UnitAIDecision.Relocate)
        {

            List<GridTile> tiles = GridParameters.LevelGrid.GetTilesWithCover(unit.transform.position, 10f);
            tiles.Shuffle();

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].Owner != null) continue;

                PointData data = new PointData();

                data.Position = GridParameters.LevelGrid.GetTileWorldPos(tiles[i]);

                if (unit.SkillController.ForceExecuteSkill(0, data, out isInstant))
                {
                    return;
                }
            }
            
            if (unit.SkillController.ForceExecuteSkill(3, null, out isInstant))
            {
                return;
            }
        }
        else if (context.Decision == UnitAIDecision.Attack)
        {
            BasicSkill skill = unit.SkillController.GetSkillByIndex(1);
            if (skill is ITargetSwitchable)
            {
                ITargetSwitchable attackSkill = skill as ITargetSwitchable;
                attackSkill.Switch(context.AttackTargetID);
                if (unit.SkillController.ForceExecuteSkill(1, null, out isInstant))
                {
                    return;
                }
            }
            if (unit.SkillController.ForceExecuteSkill(3, null, out isInstant))
            {
                return;
            }
        }
    }
}
