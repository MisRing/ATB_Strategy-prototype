using System;
using UnityEngine;
using System.Collections.Generic;

public static class CombatService
{
    private static readonly List<ICombat> COMBATS_ON_LEVEL = new List<ICombat>();
    
    public static void RegisterCombat(ICombat comb)
    {
        if (COMBATS_ON_LEVEL.Contains(comb)) return;
        COMBATS_ON_LEVEL.Add(comb);
    }
    
    public static void UnregisterCombat(ICombat comb)
    {
        if (!COMBATS_ON_LEVEL.Contains(comb)) return;
        COMBATS_ON_LEVEL.Remove(comb);
    }
    
    public static List<ICombat> GetCombats(Vector3 position, float range)
    {
        return GetCombats(position, range, UnitOwner.None);
    }
    
    public static List<ICombat> GetCombats(Vector3 position, float range, UnitOwner ally)
    {
        float sqrRange = range * range;

        List<ICombat> combatsOnRange = new List<ICombat>();

        foreach (ICombat comb in COMBATS_ON_LEVEL)
        {
            float sqrDist = (comb.Position - position).sqrMagnitude;

            if (sqrDist <= sqrRange && comb.Owner != ally)
            {
                combatsOnRange.Add(comb);
            }
        }

        combatsOnRange.Sort((a, b) =>
        {
            float distA = (a.Position - position).sqrMagnitude;
            float distB = (b.Position - position).sqrMagnitude;
            return distA.CompareTo(distB);
        });

        return combatsOnRange;
    }
    
    public static ICombat GetNearestCombat(Vector3 position, float range, UnitOwner ally)
    {
        ICombat combat = null;
        float distance = range;

        foreach (ICombat comb in COMBATS_ON_LEVEL)
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
