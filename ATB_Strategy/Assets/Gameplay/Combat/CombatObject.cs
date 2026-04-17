using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatObject : MonoBehaviour
{
    public virtual Vector3 Position { get => BodyParts.Body.Transform.position; }

    public virtual CombatBodyParts BodyParts { get; }

    public virtual UnitOwner Owner { get => UnitOwner.Enemy; }

    public List<CombatTarget> Targets = new List<CombatTarget>();

    private protected virtual void OnEnable()
    {
        CombatService.RegisterCombat(this);
    }
    
    private protected virtual void OnDisable()
    {
        CombatService.UnregisterCombat(this);
    }
}

[Serializable]
public struct CombatBodyParts
{
    public BodyPart Body;
    public BodyPart Head;
    public BodyPart[] OtherParts;
        
    [Serializable]
    public struct BodyPart
    {
        public Transform Transform;
        [Range(0f,1f)] public float Weight;
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