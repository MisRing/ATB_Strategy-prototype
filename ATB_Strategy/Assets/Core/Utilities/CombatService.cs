using System;
using UnityEngine;
using System.Collections.Generic;

public static class CombatService
{
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
                float percent = GetVisionPercent(unitCombat.BodyParts.Head.Transform.position, comb);
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

    private static float GetVisionPercent(Vector3 fromPos, CombatObject target)
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

    public static HitInfo CalculateHit(int accuracyPercent, int weaponAccuracyPercent, int weaponCritPercent, CombatTarget target)
    {
        HitInfo hit = new HitInfo();

        float distanceFactor = 1f;

        float hitChance = (accuracyPercent + weaponAccuracyPercent * distanceFactor) - target.Target.Dodge + (0 * 10);
        float critChance = Mathf.Clamp01(hitChance - 100) + weaponCritPercent * distanceFactor;

        hit.HitChance = Mathf.CeilToInt(Mathf.Clamp(hitChance, 1, 100));
        hit.CritChance = Mathf.CeilToInt(Mathf.Clamp(critChance, 0, 100));
        
        hit.Dealler = null;
        hit.Target = target.Target;

        return hit;

        // HitChance = (UnitAccuracy + WeaponAccuracyBonus * DistanceFactor) – EnemyDodge + (HighAdvantage * 10)
        // CritChance = Clamp01(HitChance – 100 %) + WeaponCritChance * DistanceFactor

    }
}

public struct HitInfo
{
    public CombatObject Dealler;
    public CombatObject Target;

    public int HitChance;
    public int CritChance;

    public int Damage;
    public int CritDamage;
}