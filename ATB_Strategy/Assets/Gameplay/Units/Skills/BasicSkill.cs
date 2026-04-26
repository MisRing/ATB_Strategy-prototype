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
    private protected int _skillCost = 1;
    private protected int _skillCooldown = 0;
    private protected int _skillCooldownTimer = 0;

    public int CurrentCooldown { get { return _skillCooldownTimer; } }
    public int MaxCooldown { get { return _skillCooldown; } }

    private protected UnitSkillController _skillController;
    private protected SkillData _skillData;

    public bool OnPrepare { get { return _onPrepare; } }
    private bool _onPrepare = false;
    
    public virtual void Init(UnitSkillController skillController)
    {
        _skillController = skillController;

        TurnManager.OnTurnEnded += CooldownTick;
    }

    private void OnDisable()
    {
        TurnManager.OnTurnEnded -= CooldownTick;
    }

    private void CooldownTick()
    {
        _skillCooldownTimer--;
    }

    public virtual void EnterPrepare()
    {
        if (_skillCooldownTimer > 0) return;

        _onPrepare = true;
    }

    public virtual void ExitPrepare()
    {
        _onPrepare = false;
    }

    public virtual bool CanExecute()
    {
        if (_skillCooldownTimer > 0) return false;
        return true;
    }

    public virtual bool IsOnCooldown()
    {
        return _skillCooldownTimer > 0;
    }

    public virtual void UpdateData(SkillData abilityData)
    {
        _skillData = abilityData;
    }

    public virtual bool Execute(ref int cost)
    {
        if (!CanExecute()) return false;

        cost = _skillCost;
        _skillCooldownTimer = _skillCooldown;
        return true;
    }
}
