using UnityEngine;

public class AbilityBasic : MonoBehaviour
{
    public string AbilityName = "Basic Ability (do nothing)";
    private protected UnitAbilityController _abilityController;
    private protected AbilityData _abilityData;
    public AbilityStatus Status;

    public virtual void Init(UnitAbilityController abilityController)
    {
        _abilityController = abilityController;
        Status = AbilityStatus.None;
    }

    public virtual void EnterPrepare(AbilityData abilityData)
    {
        Debug.Log("Enter prepare <" + AbilityName + ">");
        _abilityData = abilityData;
        Status = AbilityStatus.InPrepare;
    }

    public virtual void ExitPrepare()
    {
        Debug.Log("Exit prepare <" + AbilityName + ">");
        Status = AbilityStatus.None;
    }

    public virtual void UpdateData(AbilityData abilityData)
    {
        _abilityData = abilityData;
    }

    public virtual void Execute()
    {
        Debug.Log("Start executing <" + AbilityName + ">");
        Status = AbilityStatus.Executing;
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
}
