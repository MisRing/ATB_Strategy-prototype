using UnityEngine;

public static class TimeService
{
    private static float _timeSpeed = 1f;
    public static float TimeSpeedDelta { get { return _timeSpeed * Time.deltaTime; } }

    public static void SetTimeSpeed(float timeSpeed) => _timeSpeed = timeSpeed;
}
