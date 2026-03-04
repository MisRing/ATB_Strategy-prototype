using UnityEngine;
using System.Collections.Generic;
using System;

public class TurnManager : MonoBehaviour
{
    private static float _currentTime = 0f;

    public static event Action<UnitController> OnUnitEnterExitQ;
    
    private static List<UnitController> _freeUnits = new List<UnitController>();

    public static void EnterWaitingQ(UnitController unit)
    {
        if(_freeUnits.Contains(unit)) return;

        _freeUnits.Add(unit);
        TimeService.SetTimeSpeed(0);
        OnUnitEnterExitQ?.Invoke(unit);
    }

    public static void ExitWaitingQ(UnitController unit)
    {
        if (!_freeUnits.Contains(unit)) return;

        _freeUnits.Remove(unit);
        if (_freeUnits.Count == 0)
        {
            TimeService.SetTimeSpeed(1);
        }
    }

    private void Update()
    {
        _currentTime += TimeService.TimeSpeedDelta;
    }
}
