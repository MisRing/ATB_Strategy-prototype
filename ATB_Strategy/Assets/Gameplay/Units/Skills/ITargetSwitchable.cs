using System;
using UnityEngine;

public interface ITargetSwitchable
{
    public CombatTarget CurrentTarget { get; }
    public int TargetIndex { get; }
    public int TargetsCount { get; }
    event Action<CombatTarget> OnTargetSwitched;
    void Switch(int index);
}