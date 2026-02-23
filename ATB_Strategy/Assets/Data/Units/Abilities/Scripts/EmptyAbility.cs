using System;
using UnityEngine;

public class EmptyAbility : IAbility
{
    public AbilityStatus Status { get; private set; }

    AbilityType IAbility.Type => throw new NotImplementedException();

    public void Init(UnityAbilityController abilityController)
    {
        Status = AbilityStatus.Idle;
    }

    public void EnterPrepare()
    {
        Status = AbilityStatus.Preparing;

        Debug.Log("Empty ability on prepare");
    }

    public void Cancel()
    {
        Status = AbilityStatus.Idle;

        Debug.Log("Empty ability canceled");
    }

    public void Execute()
    {
        Status = AbilityStatus.Executing;

        Debug.Log("Empty ability executing");
    }
}