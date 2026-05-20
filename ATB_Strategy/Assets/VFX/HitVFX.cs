using System;
using System.Collections;
using UnityEngine;

public class HitVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    private void Awake()
    {
        UpdateAnimationSpeed(TimeService.TimeSpeed);
    }

    public void Play(Vector3 position, Vector3 direction)
    {
        transform.position = position;
        transform.LookAt(position + direction);
        
        StartCoroutine(Play(1f));
    }
    
    private IEnumerator Play(float lifeTime)
    {
        float time = 0;

        while (time < lifeTime)
        {
            time += TimeService.TimeSpeedDelta;
            yield return null;
        }
        
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        TimeService.OnTimeSpeedChanged += UpdateAnimationSpeed;
    }
    
    private void OnDisable()
    {
        TimeService.OnTimeSpeedChanged -= UpdateAnimationSpeed;
    }
    
    private void UpdateAnimationSpeed(float timeSpeed)
    {
        var pSys = _particleSystem.main;
        pSys.simulationSpeed = timeSpeed;
        //_particleSystem.main = pSys;
    }
}
