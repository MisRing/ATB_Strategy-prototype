using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public static readonly float TurnTime = 0.2f;
    private static float _currentTime = 0f;
    private static float _currentTurnTime = 0f;
    
    public static List<UnitComponent> Units = new List<UnitComponent>();

    private void Update()
    {
        if (CheckUnitsWaiting())
        {
            TimeService.SetTimeSpeed(0);
        }
        else
        {
            TimeService.SetTimeSpeed(1);
        }
        
        _currentTime += TimeService.TimeSpeedDelta;
        _currentTurnTime += TimeService.TimeSpeedDelta;

        if(_currentTurnTime >= TurnTime)
        {
            _currentTurnTime %= TurnTime;
            SetTurn();
        }
    }

    private bool CheckUnitsWaiting()
    {
        foreach (UnitComponent unit in Units)
        {
            if (unit.State == UnitState.WaitingForOrder)
            {
                return true;
            }
        }
        return false;
    }

    private void SetTurn()
    {

    }
}
