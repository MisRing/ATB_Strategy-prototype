using System;
using UnityEngine;

public interface ITargetSwitchable
{
    GameObject CurrentTarget { get; }
    int TargetIndex { get; }
    int TargetsCount { get; }
    event Action<GameObject> OnTargetSwitched;
    void Switch(int index);
}