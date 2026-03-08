using UnityEngine;

public class AbilityBasic : MonoBehaviour
{
    public string AbilityName = "Basic Ability (do nothing)";
    private protected UnitAbilityController _abilityController;
    private protected AbilityData _abilityData;
    public bool OnPrepare = false;

    public virtual void Init(UnitAbilityController abilityController)
    {
        _abilityController = abilityController;
        OnPrepare = false;
    }

    public virtual void EnterPrepare(AbilityData abilityData)
    {
        Debug.Log("Enter prepare <" + AbilityName + ">");
        _abilityData = abilityData;
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
        return true;
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
