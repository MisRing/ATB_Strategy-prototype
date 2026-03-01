using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static readonly float TurnTime = 0.2f;
    private static float _currentTime = 0f;
    private static float _currentTurnTime = 0f;

    private void Update()
    {
        _currentTime += TimeService.TimeSpeedDelta;
        _currentTurnTime += TimeService.TimeSpeedDelta;

        if(_currentTurnTime >= TurnTime)
        {
            _currentTurnTime %= TurnTime;
            SetTurn();
        }
    }

    private void SetTurn()
    {

    }
}
