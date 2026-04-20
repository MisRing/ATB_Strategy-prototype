using System;
using UnityEngine;

public interface ITargetSwitchable
{
    HitInfo CurrentTarget { get; set; }
    public int TargetIndex { get; }
    public int TargetsCount { get; }
    event Action<HitInfo> OnTargetSwitched;
    void Switch(int index);
}