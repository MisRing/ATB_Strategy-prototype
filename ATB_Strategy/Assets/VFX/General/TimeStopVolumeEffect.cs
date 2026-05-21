using UnityEngine;
using UnityEngine.Rendering;

public class TimeStopVolumeEffect : MonoBehaviour
{
    private Volume _volume;

    private void Awake()
    {
        _volume = GetComponent<Volume>();
        SetVolumeWeight(TimeService.TimeSpeed);
    }

    private void OnEnable()
    {
        TimeService.OnTimeSpeedChanged += SetVolumeWeight;
    }

    private void OnDisable()
    {
        TimeService.OnTimeSpeedChanged -= SetVolumeWeight;
    }

    private void SetVolumeWeight(float timeSpeed)
    {
        _volume.weight = 1f - timeSpeed;
    }
}
