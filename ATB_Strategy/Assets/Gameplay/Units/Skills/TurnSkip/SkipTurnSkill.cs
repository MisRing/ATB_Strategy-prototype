using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
        Debug.Log("Start executing <" + SkillName + "> | Cost: " + _skipTurnCount);

        cost = _skipTurnCount;

        return true;
    }
}
