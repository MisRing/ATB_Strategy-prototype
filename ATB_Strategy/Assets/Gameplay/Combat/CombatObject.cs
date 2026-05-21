using System;
using UnityEngine;

public abstract class CombatObject : MonoBehaviour
{
    public virtual CombatBodyParts BodyParts { get; }
    
    public virtual Vector3 Position { get => transform.position; }
    
    public virtual UnitOwner Owner { get => UnitOwner.EnemyTeam0; }

    private protected virtual void OnEnable()
    {
        CombatManager.RegisterCombat(this);
    }
    
    private protected virtual void OnDisable()
    {
        CombatManager.UnregisterCombat(this);
    }

    public virtual int GetDodge(Vector3 dealerPosition)
    {
        return 0;
    }

    public virtual void GetDamage(HitResult result)
    {
        Debug.Log("Someone deal me " + result.Damage + " damage");
    }
}

[Serializable]
public struct CombatBodyParts
{
    public Transform Body;
    public Transform Head;
}