using System.Collections.Generic;
using UnityEngine;

public class UnitAIController : MonoBehaviour
{
    private UnitController _unit;

    // ====== BEHAVIOR STATE ======
    private int _agression = 5;
    private int _deffensive = 1;

    public void Init()
    {
        _unit = GetComponent<UnitController>();
    }

    public UnitAIContext GetDecision(List<AITarget> allTargets)
    {
        //int targetID;
        //int attackScore = EvaluateAttack(out targetID);
        int moveScore = EvaluateMove(out Vector3 movePos, allTargets);

        //if (attackScore > moveScore && targetID != -1)
        //{
        //    return new UnitAIContext
        //    {
        //        Decision = UnitAIDecision.Attack,
        //        AttackTargetID = targetID
        //    };
        //}

        if (moveScore > 0)
        {
            return new UnitAIContext
            {
                Decision = UnitAIDecision.Relocate,
                TargetPosition = movePos
            };
        }

        return new UnitAIContext
        {
            Decision = UnitAIDecision.None
        };
    }

    private int EvaluateMove(out Vector3 movePos, List<AITarget> allTargets)
    {
        movePos = Vector3.zero;
        float range = _unit.Stats.VisionRange * 0.75f;
        List<GridTile> tiles = GridParameters.LevelGrid.GetTilesAround(transform.position, range);

        Vector3Int currentTilePos = _unit.Agent.CurrentTile;
        GridTile currentTile = GridParameters.LevelGrid.GetTile(currentTilePos.x, currentTilePos.z, currentTilePos.y);
        int score = EvaluateCurrentTile(currentTile, out movePos, allTargets);
        bool stay = true;

        foreach (GridTile tile in tiles)
        {
            Vector3 tilePos;
            int newScore = EvaluateTile(tile, out tilePos, allTargets);

            if(newScore > score)
            {
                score = newScore;
                movePos = tilePos;
                stay = false;
            }
        }
        if(stay)
        {
            return -100;
        }

        return score;
    }

    private int EvaluateCurrentTile(GridTile tile, out Vector3 tilePos, List<AITarget> allTargets)
    {
        tilePos = GridParameters.LevelGrid.GetTileWorldPos(tile);

        int deffenceScore = 0;
        int attackScore = 0;

        foreach (AITarget target in allTargets)
        {
            int deffence = CombatService.CalculateCoverDodge(tile, _unit.Stats.Dodge, target.Position);
            int tiledeffenceScore = GetDeffenceScore(deffence);
            if (Vector3.Distance(tilePos, target.Position) > _unit.Stats.VisionRange)
            {
                tiledeffenceScore /= 2;
            }
            deffenceScore += tiledeffenceScore;

            if (Vector3.Distance(tilePos, target.Position) > _unit.Stats.VisionRange) continue;

            float distance = Vector3.Distance(tilePos, target.Position);
            float distanceFactor = CombatService.CalculateDistanceFactor(distance, _unit.Combat.Weapon.RangeType);
            attackScore += Mathf.CeilToInt(50 * distanceFactor) * target.Priority;
        }

        deffenceScore *= _deffensive;
        attackScore *= _agression;

        int score = Mathf.CeilToInt((deffenceScore + attackScore));// * 2);
        return score;
    }

    private int EvaluateTile(GridTile tile, out Vector3 tilePos, List<AITarget> allTargets)
    {
        tilePos = GridParameters.LevelGrid.GetTileWorldPos(tile);
        int pathCost;
        if (!_unit.Agent.CheckPath(tilePos, out pathCost)) return -999999;
        if (pathCost == 0) return -999999;

        int deffenceScore = 0;
        int attackScore = 0;

        foreach (AITarget target in allTargets)
        {
            int deffence = CombatService.CalculateCoverDodge(tile, _unit.Stats.Dodge, target.Position);
            int tiledeffenceScore = GetDeffenceScore(deffence);
            if (Vector3.Distance(tilePos, target.Position) > _unit.Stats.VisionRange)
            {
                tiledeffenceScore /= 2;
            }
            deffenceScore += tiledeffenceScore;

            if (Vector3.Distance(tilePos, target.Position) > _unit.Stats.VisionRange) continue;

            float distance = Vector3.Distance(tilePos, target.Position);
            float distanceFactor = CombatService.CalculateDistanceFactor(distance, _unit.Combat.Weapon.RangeType);
            attackScore += Mathf.CeilToInt(50 * distanceFactor) * target.Priority;
        }

        deffenceScore *= _deffensive;
        attackScore *= _agression;

        int score = Mathf.CeilToInt((deffenceScore + attackScore));// * (8f / (float)pathCost));
        return score;
    }

    private int GetDeffenceScore(int deffence)
    {
        if(deffence >= 100)
        {
            return +100;
        }
        if(deffence >= 80)
        {
            return +50;
        }
        if (deffence >= 60)
        {
            return +20;
        }
        if (deffence >= 40)
        {
            return +0;
        }
        if (deffence >= 20)
        {
            return -40;
        }

        return -100;
    }
}

// ===================== DATA =====================

public struct UnitAIContext
{
    public UnitAIDecision Decision;
    public Vector3 TargetPosition;
    public int AttackTargetID;
}

public enum UnitAIDecision
{
    None,
    Attack,
    Relocate,
}