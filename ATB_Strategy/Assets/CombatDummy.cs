using UnityEngine;

public class CombatDummy : CombatObject
{
    public override CombatBodyParts BodyParts {get => _bodyParts; }

    [SerializeField] private CombatBodyParts _bodyParts;
}
