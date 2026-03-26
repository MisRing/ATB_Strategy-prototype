using UnityEngine;

public class AbilityBasic : MonoBehaviour
{
    public string AbilityName = "Basic Ability (do nothing)";
    public Sprite AbilityIcon;
    private protected UnitAbilityController _abilityController;
    private protected AbilityData _abilityData;
    public bool OnPrepare = false;
    
    public  UnitController Unit => _abilityController.Unit;

    public virtual void Init(UnitAbilityController abilityController)
    {
        _abilityController = abilityController;
        OnPrepare = false;
    }

    public virtual void EnterPrepare()
    {
        Debug.Log("Enter prepare <" + AbilityName + ">");
        OnPrepare = true;
    }

    public virtual void ExitPrepare()
    {
        Debug.Log("Exit prepare <" + AbilityName + ">");
        OnPrepare = false;
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

public enum AbilityStatus
{
    None,
    InPrepare,
    Executing
}

public class AbilityData
{
    public Vector3 TargetWorldPos;
    public GridTile TargetTile;
}
