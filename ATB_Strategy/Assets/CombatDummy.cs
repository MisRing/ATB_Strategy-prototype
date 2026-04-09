using UnityEngine;

public class CombatDummy : CombatAbstract
{
    public override ICombat.BodyPart[] BodyParts {get => _bodyParts; }

    [SerializeField] private ICombat.BodyPart[] _bodyParts;
}
