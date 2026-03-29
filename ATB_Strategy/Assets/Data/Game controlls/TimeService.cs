using UnityEngine;
using System;

public static class TimeService
{
    private static float _timeSpeed = 0f;

    public static readonly float DefaultTimeSpeed = 1f;
    public static float TimeSpeed { get { return _timeSpeed; } }
    public static float TimeSpeedDelta { get { return _timeSpeed * Time.deltaTime; } }
    
    public static event Action<float> OnTimeSpeedChanged;

    public static void SetTimeSpeed(float timeSpeed)
    {
        _timeSpeed = timeSpeed;
        OnTimeSpeedChanged?.Invoke(_timeSpeed);
    }

    private static bool _isPaused = false;
    private static float _savedTimeSpeed;
    public static void GamePause(bool pause)
    {
        if(_isPaused == pause) return;

        _isPaused = pause;

        if(_isPaused)
        {
            _savedTimeSpeed = _timeSpeed;
            SetTimeSpeed(0f);
        }
        else
        {
            SetTimeSpeed(_savedTimeSpeed);
        }
    }
}
