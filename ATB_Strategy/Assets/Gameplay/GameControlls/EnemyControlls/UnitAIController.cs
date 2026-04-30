using UnityEngine;

public class UnitAIController : MonoBehaviour
{
    private UnitController _unit;

    private const int BASE_ATTACK_WEIGHT = 20;
    private const int BASE_RELOCATE_WEIGHT = 20;

    private const int NO_TARGET_PENALTY = -100;
    private const int BAD_ATTACK_PENALTY = -40;

    private const int HIGH_DEFENCE = 80;
    private const int MID_DEFENCE = 50;

    private const int NEED_RELOCATE_BONUS = 100;
    private const int MID_RELOCATE_BONUS = 50;
    private const int LOW_RELOCATE_BONUS = 10;

    private const int RANDOM_VARIATION = 10;

    public void Init()
    {
        _unit = GetComponent<UnitController>();
    }

    public UnitAIContext GetDecision()
    {
        int targetID;

        int attackWeight = EvaluateAttack(out targetID);
        int relocateWeight = EvaluateRelocate();

        attackWeight += Random.Range(-RANDOM_VARIATION, RANDOM_VARIATION);
        relocateWeight += Random.Range(-RANDOM_VARIATION, RANDOM_VARIATION);

        return BuildDecision(attackWeight, relocateWeight, targetID);
    }

    // ===================== ATTACK =====================

    private int EvaluateAttack(out int bestTargetID)
    {
        bestTargetID = -1;

        if (_unit.Combat.Targets.Count == 0)
            return NO_TARGET_PENALTY;

        BasicSkill skill = _unit.SkillController.GetSkillByIndex(1);

        if (skill is not ITargetSwitchable attackSkill)
            return NO_TARGET_PENALTY;

        int bestScore = int.MinValue;

        for (int i = 0; i < _unit.Combat.Targets.Count; i++)
        {
            attackSkill.Switch(i);

            var ctx = attackSkill.SelectedTargetContext;

            int score = CalculateHitScore(ctx);

            if (score > bestScore)
            {
                bestScore = score;
                bestTargetID = i;
            }
        }

        return BASE_ATTACK_WEIGHT + ScoreAttack(bestScore);
    }

    private int CalculateHitScore(CombatContext ctx)
    {
        return ctx.HitChance + ctx.CritChance * 4;
    }

    private int ScoreAttack(int hitScore)
    {
        if (hitScore > 100) return 100;
        if (hitScore > 80) return 60;
        if (hitScore > 60) return 20;

        return BAD_ATTACK_PENALTY;
    }

    // ===================== RELOCATE =====================

    private int EvaluateRelocate()
    {
        if (_unit.Combat.Targets.Count == 0)
            return 0;

        int weight = BASE_RELOCATE_WEIGHT;

        GridTile tile = GetCurrentTile();

        foreach (var target in _unit.Combat.Targets)
        {
            int defence = CombatService.CalculateCoverDodge(
                tile,
                _unit.Stats.Dodge,
                target.Target.Position
            );

            weight += ScoreRelocate(defence);
        }

        return weight;
    }

    private int ScoreRelocate(int defence)
    {
        if (defence <= _unit.Stats.Dodge)
            return NEED_RELOCATE_BONUS;

        if (defence >= HIGH_DEFENCE)
            return LOW_RELOCATE_BONUS;

        if (defence >= MID_DEFENCE)
            return MID_RELOCATE_BONUS;

        return 0;
    }

    // ===================== FINAL DECISION =====================

    private UnitAIContext BuildDecision(int attackWeight, int relocateWeight, int targetID)
    {
        UnitAIContext context = new UnitAIContext
        {
            AttackTargetID = targetID,
            Decision = UnitAIDecision.None
        };

        if (attackWeight <= 0 && relocateWeight <= 0)
            return context;

        if (attackWeight > relocateWeight && targetID != -1)
        {
            context.Decision = UnitAIDecision.Attack;
        }
        else
        {
            context.Decision = UnitAIDecision.Relocate;
        }

        return context;
    }

    // ===================== HELPERS =====================

    private GridTile GetCurrentTile()
    {
        Vector3Int pos = _unit.Agent.CurrentTile;
        return GridParameters.LevelGrid.GetTile(pos.x, pos.z, pos.y);
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