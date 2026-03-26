using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SkipTurnAbility : AbilityBasic
{
    [SerializeField] private int _skipTurnCount = 2;
    public override void Init(UnitAbilityController abilityController)
    {
        base.Init(abilityController);
        AbilityName = "Skip";
    }

    public override bool Execute()
    {
        Debug.Log("Start executing <" + AbilityName + "> | Cost: " + _skipTurnCount);

        TurnManager.EnterBusyQ(this, _skipTurnCount);

        return true;
    }
}
