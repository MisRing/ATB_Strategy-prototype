using System;
using UnityEngine;

public interface ITargetSwitchable
{
    CombatContext SelectedTargetContext { get; set; }
    public int TargetIndex { get; }
    public int TargetsCount { get; }
    void Switch(int index);
}