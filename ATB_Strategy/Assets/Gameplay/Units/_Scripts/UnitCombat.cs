using UnityEngine;

public class UnitCombat : CombatObject
{
    public override CombatBodyParts BodyParts { get => _bodyParts; }
    [SerializeField] private CombatBodyParts _bodyParts;

    public override UnitOwner Owner { get => _unitController.Owner; }
    
    private UnitController _unitController;

    public void Init(UnitController unit)
    {
        _unitController = unit;
    }
}
