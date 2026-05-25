using UnityEngine;

public class SkipTurnSkill : BasicSkill
{
    public override void Init(UnitSkillController skillController)
    {
        base.Init(skillController);
        //SkillName = "Skip";
        _skillCost = 1;
        _skillCooldown = 0;
    }

    public override bool Execute(out int cost, ref UnitState state)
    {
        cost = _skillCost;

        if (!CanExecute()) return false;
        state = UnitState.Waiting;
        _skillCooldownTimer = _skillCooldown;
        return true;
    }
}
