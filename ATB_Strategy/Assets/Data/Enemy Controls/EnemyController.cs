using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private List<UnitController> _units = new List<UnitController>();
    [SerializeField] private List<Vector3Int> _positionPresset = new List<Vector3Int>();
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (GridParameters.LevelGrid == null)
        {
            GridParameters.LevelGrid = FindFirstObjectByType(typeof(GridMap)) as GridMap;
        }

        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].Init(GridParameters.LevelGrid.GetTile(_positionPresset[i].x, _positionPresset[i].z, _positionPresset[i].y));

            _units[i].SkillController.OnSkillFinished += StartUnitAbility;
            StartUnitAbility(_units[i]);
        }
    }

    private void StartUnitAbility(UnitController unit)
    {
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

            if (unit.SkillController.ForceExecuteSkill(-1, data))
            {
                break;
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if(GridParameters.LevelGrid == null)
        {
            GridParameters.LevelGrid = FindFirstObjectByType(typeof(GridMap)) as GridMap;
        }

        foreach(Vector3Int point in _positionPresset)
        {
            if(GridParameters.LevelGrid.CheckTile(point.x, point.z, point.y))
            {
                Gizmos.color = Color.darkRed;
                Gizmos.DrawSphere(GridParameters.LevelGrid.GetTileWorldPos(point.x, point.z, point.y), 0.3f);
            }
        }
    }
}
