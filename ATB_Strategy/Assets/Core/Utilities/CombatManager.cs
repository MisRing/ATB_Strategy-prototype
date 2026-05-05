using System;
using UnityEngine;
using System.Collections.Generic;

public static class CombatManager
{
    private static readonly List<CombatObject> COMBATS_ON_LEVEL = new List<CombatObject>();
    private static readonly LayerMask ENVIRONMENT_MASK = LayerMask.GetMask("Grid Environment");
    
    private static bool _needToResetVisibility = false;

    public static event Action OnVisibilityChanged;
    
    public static void TriggerVisibilityReset()
    {
        _needToResetVisibility = true;
    }

    public static void ResetVisibility()
    {
        if (!_needToResetVisibility) return;

        _needToResetVisibility = false;
        OnVisibilityChanged?.Invoke();
    }
    
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

    public static List<CombatObject> GetAllCombats(UnitOwner ally)
    {
        List<CombatObject> combats = new List<CombatObject>();
        foreach (CombatObject comb in COMBATS_ON_LEVEL)
        {
            if (comb.Owner == ally) continue;

            combats.Add(comb);
        }
        return combats;
    }


    public static List<CombatTarget> GetCombats(CombatObject unitCombat, float range, UnitOwner ally)
    {
        float sqrRange = range * range;
        Vector3 unitPosition = unitCombat.Position;

        List<CombatTarget> combatsOnRange = new List<CombatTarget>();

        foreach (CombatObject comb in COMBATS_ON_LEVEL)
        {
            float sqrDist = (comb.Position - unitPosition).sqrMagnitude;

            if (sqrDist > sqrRange || comb.Owner == ally) continue;

            float percent = GetVisionPercent(unitCombat.Position + Vector3.up, comb);

            if (percent == 0f) continue;
            
            CombatTarget target = new CombatTarget(comb, percent);
            combatsOnRange.Add(target);

        }

        combatsOnRange.Sort((a, b) =>
        {
            float distA = (a.Target.Position - unitPosition).sqrMagnitude;
            float distB = (b.Target.Position - unitPosition).sqrMagnitude;
            return distA.CompareTo(distB);
        });

        return combatsOnRange;
    }

    public static float GetVisionPercent(Vector3 fromPos, CombatObject target)
    {
        float percent = 0;
        
        if (target.BodyParts.Body.Transform)
        {
            percent += HasLineBetween(
                fromPos,
                target.BodyParts.Body.Transform.position,
                ENVIRONMENT_MASK
                )
                ? target.BodyParts.Body.Weight
                : 0f;
        }
        
        if (target.BodyParts.Head.Transform)
        {
            percent += HasLineBetween(
                fromPos,
                target.BodyParts.Head.Transform.position,
                ENVIRONMENT_MASK
                )
                ? target.BodyParts.Head.Weight
                : 0f;
        }

        foreach (CombatBodyParts.BodyPart part in target.BodyParts.OtherParts)
        {
            percent += HasLineBetween(
                fromPos,
                part.Transform.position,
                ENVIRONMENT_MASK
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