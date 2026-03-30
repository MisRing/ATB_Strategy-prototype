using System;
using UnityEngine;

public class AbilityBasic : MonoBehaviour
{
    public string AbilityName = "Basic Ability";
    public string AbilityDescription = "This ability did nothing";
    public Sprite AbilityIcon;
    private protected UnitAbilityController _abilityController;
    private protected AbilityData _abilityData;
    
    public  UnitController Unit => _abilityController.Unit;

    public virtual void Init(UnitAbilityController abilityController)
    {
        _abilityController = abilityController;
    }

    public virtual void EnterPrepare()
    {
        Debug.Log("Enter prepare <" + AbilityName + ">");
    }

    public virtual void ExitPrepare()
    {
        Debug.Log("Exit prepare <" + AbilityName + ">");
    }

    public virtual void UpdateData(AbilityData abilityData)
    {
        _abilityData = abilityData;
    }

    public virtual bool Execute()
    {
        Debug.Log("Start executing <" + AbilityName + ">");
        TurnManager.EnterBusyQ(this, 1);
        return true;
    }
    
    public virtual void FinishExecute()
    {
        Debug.Log(_abilityController.Unit.name + " end <" + AbilityName + ">");
        _abilityController.FinishExecuteAbility();
    }
}

public class AbilityData
{
    public Vector3 TargetWorldPos;
    public GridTile TargetTile;
}
