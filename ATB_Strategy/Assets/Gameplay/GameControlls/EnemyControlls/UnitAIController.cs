using System.Collections.Generic;
using UnityEngine;

public class UnitAIController : MonoBehaviour
{
    private UnitController _unit;

    [SerializeField] private int _aggression = 5;
    [SerializeField] private int _defensive = 1;

    public void Init()
    {
        _unit = GetComponent<UnitController>();
    }
    
    public UnitAIContext GetDecision(List<AITarget> targets)
    {
        int moveScore = EvaluateMove(targets, out Vector3 movePos);
        int attackScore = EvaluateAttack(targets, out int targetID);
        
        //Debug.Log("Decision: move = " + moveScore + " | attack = " + attackScore);

        if (moveScore > 0 && attackScore < moveScore)
        {
            return new UnitAIContext
            {
                Decision = UnitAIDecision.Relocate,
                TargetPosition = movePos
            };
        }
        
        if (attackScore > 0 && attackScore >= moveScore && targetID != -1)
        {
            return new UnitAIContext
            {
                Decision = UnitAIDecision.Attack,
                AttackTargetID = targetID
            };
        }

        return UnitAIContext.None;
    }
    
    // ==================== Attack ====================

    private int EvaluateAttack(List<AITarget> targets, out int targetID)
    {
        targetID = -1;
        int score = 0;
        
        var attack = _unit.SkillController.GetSkillByIndex(1) as ITargetSwitchable;
        if (attack == null) return -100;
        
        for (int i = 0; i < _unit.Combat.Targets.Count; i++)
        {
            attack.Switch(i);
            CombatContext context = attack.SelectedTargetContext;

            int targetScore = 0;
            if(context.HitChance >= 60) targetScore += context.HitChance * 4;
            else continue;

            targetScore += context.CritChance * 5;

            int priority = targets.Find(x => x.Target == _unit.Combat.Targets[i]).Priority;
            
            targetScore *= priority;

            if (targetID == -1 || targetScore > score)
            {
                score = targetScore;
                targetID = i;
            }
        }

        score *= _aggression;
        return score;
    }

    // ===================== MOVE =====================

    private int EvaluateMove(List<AITarget> targets, out Vector3 bestPosition)
    {
        bestPosition = transform.position;

        float range = _unit.Stats.VisionRange * 0.75f;
        List<GridTile> candidates = _unit.Combat.VisibleTiles;

        int bestScore = EvaluateTileScore(_unit.Agent.CurrentTile, _unit.Agent.CurrentTile.WorldPosition, targets, ignorePath: true);

        foreach (var tile in candidates)
        {
            if (tile.Owner != null) continue;
            if (!tile.IsGround) continue;

            Vector3 pos = tile.WorldPosition;

            if (!_unit.Agent.CheckPath(pos, out int pathCost))
                continue;

            if (pathCost <= 0 || pathCost > 30)
                continue;

            int score = EvaluateTileScore(tile, pos, targets, ignorePath: false);

            if (score > bestScore)
            {
                bestScore = score;
                bestPosition = pos;
            }
        }

        if (bestPosition == _unit.Agent.CurrentTile.WorldPosition)
            return -100;

        return bestScore;
    }


    private int EvaluateTileScore(GridTile tile, Vector3 tilePos, List<AITarget> targets, bool ignorePath)
    {
        int defenceScore = 0;
        int attackScore = 0;

        float sqrVision = _unit.Stats.VisionRange * _unit.Stats.VisionRange;

        foreach (var target in targets)
        {
            Vector3 toTarget = target.Position - tilePos;
            float sqrDist = toTarget.sqrMagnitude;

            bool inVision = sqrDist <= sqrVision;

            int defence = CombatService.CalculateCoverDodge(tile, _unit.Stats.Dodge, target.Position);
            int defenceValue = GetDefenceScore(defence);

            if (!inVision)
                defenceValue /= 2;

            defenceScore += defenceValue;

            if (!inVision) continue;

            float distance = Mathf.Sqrt(sqrDist);
            float distanceFactor = CombatService.CalculateDistanceFactor(distance, _unit.Combat.Weapon.RangeType);

            attackScore += Mathf.CeilToInt(50 * distanceFactor) * target.Priority;
        }

        defenceScore *= _defensive;
        attackScore *= _aggression;

        return defenceScore + attackScore;
    }

    private int GetDefenceScore(int defence)
    {
        if (defence >= 100) return 100;
        if (defence >= 80) return 50;
        if (defence >= 60) return 20;
        if (defence >= 40) return -100;

        return -200;
    }
}


public struct UnitAIContext
{
    public UnitAIDecision Decision;
    public Vector3 TargetPosition;
    public int AttackTargetID;

    public static UnitAIContext None => new UnitAIContext
    {
        Decision = UnitAIDecision.None
    };
}

public enum UnitAIDecision
{
    None,
    Attack,
    Relocate,
}