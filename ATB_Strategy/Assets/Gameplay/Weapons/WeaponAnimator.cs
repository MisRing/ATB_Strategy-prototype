using System.Collections;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _barrelTransform;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpeed = 20f;

    public static readonly int FIRE = Animator.StringToHash("Fire");

    private void OnEnable()
    {
        TimeService.OnTimeSpeedChanged += UpdateAnimationSpeed;
    }
    
    private void OnDisable()
    {
        TimeService.OnTimeSpeedChanged -= UpdateAnimationSpeed;
    }

    public void FireAnimation()
    {
        _animator.SetTrigger(FIRE);
        StartCoroutine(ShootBullet(10f));
    }

    private IEnumerator ShootBullet(float bulletTime)
    {
        GameObject bullet = Instantiate(_bulletPrefab);
        bullet.transform.position = _barrelTransform.position;
        bullet.transform.rotation = _barrelTransform.rotation;

        float time = 0;

        while (time < bulletTime)
        {
            time += TimeService.TimeSpeedDelta;
            
            bullet.transform.position += bullet.transform.forward * TimeService.TimeSpeedDelta * _bulletSpeed;

            yield return null;
        }
        
        Destroy(bullet);
    }
    
    private void UpdateAnimationSpeed(float timeSpeed)
    {
        _animator.speed = timeSpeed;
    }
}
