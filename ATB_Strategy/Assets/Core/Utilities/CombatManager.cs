using UnityEngine;
using System.Collections.Generic;

public static class CombatManager
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


    public static List<CombatObject> GetCombats(CombatObject unitCombat, List<GridTile> visibleTiles, UnitOwner ally)
    {
        Vector3 unitPosition = unitCombat.Position;

        List<CombatObject> combatsOnRange = new List<CombatObject>();

        foreach (CombatObject comb in COMBATS_ON_LEVEL)
        {
            if (comb.Owner == ally) continue;

            GridTile combTile = GridParameters.LevelGrid.GetTileByWorldPos(comb.transform.position);
            if (visibleTiles.Contains(combTile))
            {
                combatsOnRange.Add(comb);
            }
        }

        combatsOnRange.Sort((a, b) =>
        {
            float distA = (a.Position - unitPosition).sqrMagnitude;
            float distB = (b.Position - unitPosition).sqrMagnitude;
            return distA.CompareTo(distB);
        });

        return combatsOnRange;
    }

}