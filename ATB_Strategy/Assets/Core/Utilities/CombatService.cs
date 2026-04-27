using UnityEngine;
using System.Collections.Generic;

public static class CombatService
{
    private static readonly int HEIGHT_ADVANTAGE_BONUS_ACCURACY = 5;
    
    private static readonly float MELEE_DISTANCE_THRESHHOLD = 5f;
    private static readonly float MEDIUM_DISTANCE_THRESHHOLD = 15f;
    private static readonly float RANGE_DISTANCE_THRESHHOLD = 20f;

    private static readonly float MAX_DISTANCE_FACTOR = 1f;
    private static readonly float MIN_DISTANCE_FACTOR = -0.5f;
    
    private static readonly List<CombatObject> COMBATS_ON_LEVEL = new List<CombatObject>();
    
    public static void RegisterCombat(CombatObject comb)
    {
        if (COMBATS_ON_LEVEL.Contains(comb)) return;
        COMBATS_ON_LEVEL.Add(comb);
    }
    
    public static void UnregisterCombat(CombatObject comb)
    {
        if (!COMBATS_ON_LEVEL.Contains(comb)) return;
        COMBATS_ON_LEVEL.Remove(comb);
    }
    
    public static List<CombatTarget> GetCombats(CombatObject unitCombat, float range)
    {
        return GetCombats(unitCombat, range, UnitOwner.None);
    }
    
    public static List<CombatTarget> GetCombats(CombatObject unitCombat, float range, UnitOwner ally)
    {
        float sqrRange = range * range;
        Vector3 unitPosition = unitCombat.Position;

        List<CombatTarget> combatsOnRange = new List<CombatTarget>();

        foreach (CombatObject comb in COMBATS_ON_LEVEL)
        {
            float sqrDist = (comb.Position - unitPosition).sqrMagnitude;

            if (sqrDist <= sqrRange && comb.Owner != ally)
            {
                float percent = GetVisionPercent(unitCombat.Position + Vector3.up, comb);
                if(percent == 0f) continue;
                CombatTarget target = new CombatTarget(comb, percent);
                combatsOnRange.Add(target);
            }
        }

        combatsOnRange.Sort((a, b) =>
        {
            float distA = (a.Target.Position - unitPosition).sqrMagnitude;
            float distB = (b.Target.Position - unitPosition).sqrMagnitude;
            return distA.CompareTo(distB);
        });

        return combatsOnRange;
    }
    
    public static CombatObject GetNearestCombat(Vector3 position, float range, UnitOwner ally)
    {
        CombatObject combat = null;
        float distance = range;

        foreach (CombatObject comb in COMBATS_ON_LEVEL)
        {
            if (comb.Owner == ally) continue;

            float newDistance = Vector3.Distance(position, comb.Position);

            if (newDistance <= distance)
            {
                distance = newDistance;
                combat = comb;
            }
        }

        return combat;
    }

    public static float GetVisionPercent(Vector3 fromPos, CombatObject target)
    {
        LayerMask mask = LayerMask.GetMask("Grid Environment");

        float percent = 0;
        if (target.BodyParts.Body.Transform)
        {
            percent += HasLineBetween(
                fromPos,
                target.BodyParts.Body.Transform.position,
                mask
                )
                ? target.BodyParts.Body.Weight
                : 0f;
        }
        
        if (target.BodyParts.Head.Transform)
        {
            percent += HasLineBetween(
                fromPos,
                target.BodyParts.Head.Transform.position,
                mask
                )
                ? target.BodyParts.Head.Weight
                : 0f;
        }

        foreach (CombatBodyParts.BodyPart part in target.BodyParts.OtherParts)
        {
            percent += HasLineBetween(
                fromPos,
                part.Transform.position,
                mask
                )
                ? part.Weight
                : 0f;
        }

        return percent;
    }

    private static bool HasLineBetween(Vector3 point1, Vector3 point2, LayerMask mask)
    {
        return !Physics.Linecast(point1, point2, mask);
    }

    public static CombatContext CalculateHitContext(CombatObject dealer, int accuracyPercent, WeaponController weapon, CombatTarget target)
    {
        CombatContext context = new CombatContext();

        float distance = Vector3.Distance(target.Target.Position, dealer.Position);
        float distanceFactor = CalculateDistanceFactor(distance, weapon.RangeType);
        float heightDiff = dealer.Position.y - target.Target.Position.y;
        int heightAdvantage = Mathf.FloorToInt(Mathf.Abs(heightDiff)) * (int)Mathf.Sign(heightDiff);
        
        float hitChance =
            (accuracyPercent
             + weapon.Accuracy * distanceFactor
             + (heightAdvantage * HEIGHT_ADVANTAGE_BONUS_ACCURACY))
            * target.VisionPercent
            - target.Target.Dodge;
        
        float critChance = Mathf.Clamp01(hitChance - 100) + weapon.CritChance * distanceFactor;

        context.HitChance = Mathf.CeilToInt(Mathf.Clamp(hitChance, 1, 100));
        context.CritChance = Mathf.CeilToInt(Mathf.Clamp(critChance, 0, 100));
        
        context.Dealer = dealer;
        context.Target = target;

        context.Damage = weapon.Damage;
        context.CritDamage = weapon.CritDamage;

        return context;
    }

    public static bool CalculateHit(CombatContext context, out HitResult result)
    {
        int random = Random.Range(0, 100);

        result = new HitResult();
        result.Dealer = context.Dealer;
        if (random < context.HitChance)
        {
            random = Random.Range(0, 100);
            if (random < context.CritChance)
            {
                result.IsCritical = true;
                result.Damage = Random.Range(context.CritDamage.Min, context.CritDamage.Max + 1);
            }
            else
            {
                result.IsCritical = false;
                result.Damage = Random.Range(context.Damage.Min, context.Damage.Max + 1);
            }

            return true;
        }

        return false;
    }

    private static float CalculateDistanceFactor(float distance, WeaponRangeType type)
    {
        switch (type)
        {
            case(WeaponRangeType.Melee): return CalculateDistanceFactorMelee(distance);
            case(WeaponRangeType.Medium): return CalculateDistanceFactorMedium(distance);
            case(WeaponRangeType.Ranged): return CalculateDistanceFactorRange(distance);
        }
        
        return MAX_DISTANCE_FACTOR;
    }

    private static float CalculateDistanceFactorMelee(float distance)
    {
        if (distance <= MELEE_DISTANCE_THRESHHOLD)
        {
            return MAX_DISTANCE_FACTOR;
        }
        if (distance >= MEDIUM_DISTANCE_THRESHHOLD)
        {
            return MIN_DISTANCE_FACTOR;
        }
        float f = (distance - MELEE_DISTANCE_THRESHHOLD) / (MEDIUM_DISTANCE_THRESHHOLD - MELEE_DISTANCE_THRESHHOLD);
        float distanceFactor = Mathf.Lerp(MAX_DISTANCE_FACTOR, MIN_DISTANCE_FACTOR, f);

        return distanceFactor;
    }
    
    private static float CalculateDistanceFactorMedium(float distance)
    {
        if (distance <= MELEE_DISTANCE_THRESHHOLD)
        {
            float f = distance / MELEE_DISTANCE_THRESHHOLD;
            float distanceFactor = Mathf.Lerp(MIN_DISTANCE_FACTOR, MAX_DISTANCE_FACTOR, f);

            return distanceFactor;
        }
        if (distance >= MEDIUM_DISTANCE_THRESHHOLD)
        {
            float f = (distance - MEDIUM_DISTANCE_THRESHHOLD) / (RANGE_DISTANCE_THRESHHOLD - MEDIUM_DISTANCE_THRESHHOLD);
            float distanceFactor = Mathf.Lerp(MAX_DISTANCE_FACTOR, MIN_DISTANCE_FACTOR, f);

            return distanceFactor;
        }

        return MAX_DISTANCE_FACTOR;
    }
    
    private static float CalculateDistanceFactorRange(float distance)
    {
        if (distance <= MELEE_DISTANCE_THRESHHOLD)
        {
            return MIN_DISTANCE_FACTOR;
        }
        if (distance >= MEDIUM_DISTANCE_THRESHHOLD)
        {
            return MAX_DISTANCE_FACTOR;
        }
        float f = (distance - MELEE_DISTANCE_THRESHHOLD) / (MEDIUM_DISTANCE_THRESHHOLD - MELEE_DISTANCE_THRESHHOLD);
        float distanceFactor = Mathf.Lerp(MIN_DISTANCE_FACTOR, MAX_DISTANCE_FACTOR, f);

        return distanceFactor;
    }
}

public struct CombatContext
{
    public CombatObject Dealer;
    public CombatTarget Target;

    public int HitChance;
    public int CritChance;

    public RangeIntStat Damage;
    public RangeIntStat CritDamage;
}

public struct HitResult
{
    public CombatObject Dealer;
    public int Damage; 
    public bool IsCritical;
}

public struct CombatTarget
{
    public CombatObject Target;
    public float VisionPercent;

    public CombatTarget(CombatObject target, float visionPercent)
    {
        Target = target;
        VisionPercent = visionPercent;
    }
}