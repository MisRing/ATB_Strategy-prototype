using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatObject : MonoBehaviour
{
    public virtual Vector3 Position { get => BodyParts.Body.Transform.position; }

    public virtual CombatBodyParts BodyParts { get; }

    public virtual int Dodge { get => 10; }

    public virtual UnitOwner Owner { get => UnitOwner.Enemy; }

    private protected virtual void OnEnable()
    {
        CombatService.RegisterCombat(this);
    }
    
    private protected virtual void OnDisable()
    {
        CombatService.UnregisterCombat(this);
    }

    public virtual void GetDamage(HitResult result)
    {
        Debug.Log("Someone deal me " + result.Damage + " damage");
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