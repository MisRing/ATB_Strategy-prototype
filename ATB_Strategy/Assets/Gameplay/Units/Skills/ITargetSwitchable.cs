using System;
using UnityEngine;

public interface ITargetSwitchable
{
    CombatContext CurrentTarget { get; set; }
    public int TargetIndex { get; }
    public int TargetsCount { get; }
    event Action<CombatContext> OnTargetSwitched;
    void Switch(int index);
}