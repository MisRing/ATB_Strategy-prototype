using UnityEngine;

public class SkipTurnSkill : BasicSkill
{
    [SerializeField] private int _skipTurnCount = 2;
    public override void Init(UnitSkillController skillController)
    {
        base.Init(skillController);
        SkillName = "Skip";
    }

    public override bool Execute(ref int cost)
    {
        cost = _skipTurnCount;

        return true;
    }
}
