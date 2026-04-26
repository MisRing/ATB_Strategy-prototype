using UnityEngine;

public class SkipTurnSkill : BasicSkill
{
    public override void Init(UnitSkillController skillController)
    {
        base.Init(skillController);
        SkillName = "Skip";
        _skillCost = 2;
        _skillCooldown = 0;
    }
}
