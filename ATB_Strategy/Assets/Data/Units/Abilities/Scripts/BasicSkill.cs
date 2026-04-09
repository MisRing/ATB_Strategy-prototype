using System;
using UnityEngine;

public class BasicSkill : MonoBehaviour
{
    [Header("Info parameters")]
    public string SkillName = "Basic Skill";
    public string SkillDescription = "This skill did nothing";
    public Sprite SkillIcon;

    [Header("Skill parameters")]
    public Type RequiredDataType = null;
    
    private protected UnitSkillController _skillController;
    private protected SkillData _skillData;
    
    public virtual void Init(UnitSkillController skillController)
    {
        _skillController = skillController;
    }

    public virtual void EnterPrepare()
    {
        Debug.Log("Enter prepare <" + SkillName + ">");
    }

    public virtual void ExitPrepare()
    {
        Debug.Log("Exit prepare <" + SkillName + ">");
    }

    public virtual void UpdateData(SkillData abilityData)
    {
        _skillData = abilityData;
    }

    public virtual bool Execute(ref int cost)
    {
        Debug.Log("Start executing <" + SkillName + ">");
        cost = 1;
        return true;
    }
}

public abstract class SkillData { }

public class TargetData : SkillData
{
    public GameObject Target;
}

public class PointData : SkillData
{
    public Vector3 Position;
}

public interface ITargetSwitchable
{
    GameObject CurrentTarget { get; }
    event Action<GameObject> OnTargetSwitched;
    void Switch();
}
