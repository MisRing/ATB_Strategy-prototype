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
                CombatTarget target = new CombatTarget(comb, 1f);
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
}
