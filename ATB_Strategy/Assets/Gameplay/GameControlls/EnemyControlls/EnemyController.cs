using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private List<UnitController> _units = new List<UnitController>();


    public void Init(List<UnitController> units)
    {
        _units = units;

        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].SkillController.OnSkillFinished += StartUnitAbility;
            StartUnitAbility(_units[i]);
        }
    }

    private void StartUnitAbility(UnitController unit)
    {
        if (unit.SkillController.ForceExecuteSkill(2, null, out bool isInstant))
        {
            return;
        }
        
        while (true)
        {
            PointData data = new PointData();
            Vector3 randomTarget = new Vector3(
                Random.Range(5f, 15f) * (Random.Range(0, 2) * 2 - 1),
                0f,
                Random.Range(5f, 15f) * (Random.Range(0, 2) * 2 - 1)
                );
            randomTarget += unit.transform.position;
            data.Position = randomTarget;
            GridTile tile = new GridTile();
            if (!GridParameters.LevelGrid.GetTileByWorldPos(ref tile, randomTarget))
            {
                continue;
            }
            if (tile.Owner)
            {
                continue;
            }

            //if (unit.SkillController.ForceExecuteSkill(0, data, out bool isInstant))
            //{
            //    return;
            //}
        }
    }
}
