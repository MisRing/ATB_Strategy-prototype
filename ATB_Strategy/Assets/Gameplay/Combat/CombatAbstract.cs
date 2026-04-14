using UnityEngine;

public abstract class CombatAbstract : MonoBehaviour, ICombat
{
    public virtual Vector3 Position { get => BodyParts[0].transform.position; }

    public virtual ICombat.BodyPart[] BodyParts { get; }

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