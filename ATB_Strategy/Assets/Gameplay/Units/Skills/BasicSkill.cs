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

    public bool OnPrepare { get { return _onPrepare; } }
    private bool _onPrepare = false;
    
    public virtual void Init(UnitSkillController skillController)
    {
        _skillController = skillController;
    }

    public virtual void EnterPrepare()
    {
        _onPrepare = true;
    }

    public virtual void ExitPrepare()
    {
        _onPrepare = false;
    }

    public virtual void UpdateData(SkillData abilityData)
    {
        _skillData = abilityData;
    }

    public virtual bool Execute(ref int cost)
    {
        cost = 1;
        return true;
    }
}
