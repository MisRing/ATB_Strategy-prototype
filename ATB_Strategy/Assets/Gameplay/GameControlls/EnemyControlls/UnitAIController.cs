using System.Collections.Generic;
using UnityEngine;

public class UnitAIController : MonoBehaviour
{
    private UnitController _unit;

    // ====== BEHAVIOR STATE ======
    private float _aggression = 0f;   // >0 = агрессивный
    private float _defensiveness = 0f; // >0 = осторожный

    private const float STATE_DECAY = 1f;

    // ====== WEIGHTS ======
    private const int BASE_ATTACK = 10;
    private const int BASE_MOVE = 20;

    private const int RANDOM = 20;

    private const float IDEAL_DISTANCE = 6f;

    public void Init()
    {
        _unit = GetComponent<UnitController>();
    }

    public UnitAIContext GetDecision(List<AITarget> allTargets)
    {
        DecayState();

        int targetID;
        int attackScore = -1000000;
        EvaluateAttack(out targetID); // off
        int moveScore = EvaluateMove(out Vector3 movePos, allTargets);

        //attackScore += Random.Range(-RANDOM, RANDOM);
        //moveScore += Random.Range(-RANDOM, RANDOM);

        // влияние поведения
        attackScore += Mathf.RoundToInt(_aggression * 50f);
        moveScore += Mathf.RoundToInt(_defensiveness * 50f);

        Debug.LogWarning(moveScore);

        if (attackScore > moveScore && targetID != -1)
        {
            return new UnitAIContext
            {
                Decision = UnitAIDecision.Attack,
                AttackTargetID = targetID
            };
        }

        if (movePos != Vector3.zero)
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

    // ===================== ATTACK =====================

    private int EvaluateAttack(out int bestTargetID)
    {
        bestTargetID = -1;

        if (_unit.Combat.Targets.Count == 0)
            return -100;

        BasicSkill skill = _unit.SkillController.GetSkillByIndex(1);

        if (skill is not ITargetSwitchable attackSkill)
            return -100;

        int bestScore = -999;

        for (int i = 0; i < _unit.Combat.Targets.Count; i++)
        {
            attackSkill.Switch(i);
            var ctx = attackSkill.SelectedTargetContext;

            int score = ctx.HitChance + ctx.CritChance * 4;

            if (score > bestScore)
            {
                bestScore = score;
                bestTargetID = i;
            }
        }

        return BASE_ATTACK + ScoreAttack(bestScore);
    }

    private int ScoreAttack(int hit)
    {
        if (hit > 100) return 100;
        if (hit > 80) return 60;
        if (hit > 60) return 40;
        if (hit > 40) return 20;
        return -40;
    }

    // ===================== MOVE =====================

    private int EvaluateMove(out Vector3 bestPos, List<AITarget> allTargets)
    {
        bestPos = Vector3.zero;
        float radius = _unit.Stats.VisionRange * 0.75f;
        List<GridTile> tiles = GridParameters.LevelGrid.GetTilesWithCover(_unit.transform.position, radius);
        tiles.Shuffle();

        int bestScore = -999;

        Vector3Int currentTilePos = _unit.Agent.CurrentTile;
        GridTile currentTile = GridParameters.LevelGrid.GetTile(currentTilePos.x, currentTilePos.z, currentTilePos.y);
        Vector3 tilePos = GridParameters.LevelGrid.GetTileWorldPos(currentTile);
        int currentScore = EvaluateTile(currentTile, tilePos, allTargets);

        foreach (GridTile tile in tiles)
        {
            if (tile.Owner != null) continue;

            Vector3 pos = GridParameters.LevelGrid.GetTileWorldPos(tile);
            if (tilePos == pos) continue;

            int score = EvaluateTile(tile, pos, allTargets);

            if (score > bestScore)
            {
                bestScore = score;
                bestPos = pos;
            }
        }

        if(currentScore < bestScore)
        {
            return BASE_MOVE;
        }

        if(currentScore < 10)
        {
            bestScore += 100;
        }

        return BASE_MOVE + bestScore;
    }

    private int EvaluateTile(GridTile tile, Vector3 pos, List<AITarget> targets)
    {
        int score = 0;

        foreach (var target in targets)
        {
            Vector3 enemyPos = target.Position;

            // ===== DEFENCE =====
            int defence = CombatService.CalculateCoverDodge(tile, _unit.Stats.Dodge, enemyPos);

            if (defence <= _unit.Stats.Dodge)
                score -= 200;
            else if (defence > 80)
                score += 40;
            else if (defence > 60)
                score -= 100;
            else if (defence > 40)
                score -= 800;
            else
                score -= -1200;

            // ===== DISTANCE =====
            float dist = Vector3.Distance(pos, enemyPos);
            float distScore = Mathf.Abs(dist - IDEAL_DISTANCE);

            score -= Mathf.RoundToInt(distScore * 5f);
        }

        return score;
    }

    // ===================== BEHAVIOR CONTROL =====================

    public void ApplyAggression(float value)
    {
        _aggression += value;
    }

    public void ApplyDefensiveness(float value)
    {
        _defensiveness += value;
    }

    private void DecayState()
    {
        _aggression = Mathf.MoveTowards(_aggression, 0f, STATE_DECAY * Time.deltaTime);
        _defensiveness = Mathf.MoveTowards(_defensiveness, 0f, STATE_DECAY * Time.deltaTime);
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