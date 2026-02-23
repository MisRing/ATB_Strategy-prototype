using System;
using UnityEngine;

public interface IAbility
{
    AbilityType Type {get; }
    AbilityStatus Status { get; }

    void Init(UnityAbilityController abilityController);

    void EnterPrepare();
    void Execute();
    void Cancel();
}

public enum AbilityStatus
{
    Idle,
    Preparing,
    Executing,
    Finished
}

public enum AbilityType
{
    None,
    Targeted,
    Instant
}
