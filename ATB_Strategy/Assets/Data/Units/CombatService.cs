using System;
using UnityEngine;
using System.Collections;
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
        List<ICombat> combatsOnRange = new List<ICombat>();

        foreach (ICombat comb in COMBATS_ON_LEVEL)
        {
            if (Vector3.Distance(position, comb.Position) <= range)
            {
                combatsOnRange.Add(comb);
            }
        }

        return combatsOnRange;
    }
    
    public static List<ICombat> GetCombats(Vector3 position, float range, UnitOwner ally)
    {
        List<ICombat> combatsOnRange = new List<ICombat>();

        foreach (ICombat comb in COMBATS_ON_LEVEL)
        {
            if (Vector3.Distance(position, comb.Position) <= range && comb.Owner != ally)
            {
                combatsOnRange.Add(comb);
            }
        }

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

public interface ICombat
{
    Vector3 Position { get; }
    UnitOwner Owner { get; }
}

public abstract class CombatAbstract : MonoBehaviour, ICombat
{
    public virtual Vector3 Position { get => transform.position; }
    public virtual UnitOwner Owner { get => UnitOwner.Enemy; }

    private protected virtual void OnEnable()
    {
        CombatService.RegisterCombat(this);
    }
    
    private protected virtual void OnDisable()
    {
        CombatService.UnregisterCombat(this);
    }
}
