using UnityEngine;

public class UnitCombat : CombatAbstract
{
    public override UnitOwner Owner { get => _unitController.Owner; }

    public override ICombat.BodyPart[] BodyParts {get => _bodyParts; }

    [SerializeField] private ICombat.BodyPart[] _bodyParts;


    private UnitController _unitController;

    public void Init(UnitController unit)
    {
        _unitController = unit;
    }
}
