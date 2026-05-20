using System.Collections;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _barrelTransform;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _bulletSpeed = 20f;
    [SerializeField] private float _missOffset = 1f;

    public static readonly int FIRE = Animator.StringToHash("Fire");

    private void OnEnable()
    {
        TimeService.OnTimeSpeedChanged += UpdateAnimationSpeed;
    }
    
    private void OnDisable()
    {
        TimeService.OnTimeSpeedChanged -= UpdateAnimationSpeed;
    }

    public float FireAnimation(Transform target, bool hit)
    {
        _animator.SetTrigger(FIRE);
        StartCoroutine(ShootBullet(5f, target, hit));

        float timeToHit = (transform.position - target.position).magnitude / _bulletSpeed;

        return timeToHit;
    }

    private IEnumerator ShootBullet(float bulletTime, Transform target,  bool hit)
    {
        Vector3 offset = hit
            ? Vector3.zero
            : new Vector3(
                _missOffset * (Random.Range(0, 1) * 2 - 1),
                _missOffset * (Random.Range(0, 1) * 2 - 1),
                _missOffset * (Random.Range(0, 1) * 2 - 1)
                );
        
        GameObject bullet = Instantiate(_bulletPrefab);
        bullet.transform.position = _barrelTransform.position;
        bullet.transform.rotation = _barrelTransform.rotation;

        float time = 0;

        while (time < bulletTime)
        {
            time += TimeService.TimeSpeedDelta;
            Vector3 direction = (target.position + offset - transform.position).normalized;
            bullet.transform.position += direction * TimeService.TimeSpeedDelta * _bulletSpeed;

            yield return null;
        }
        
        Destroy(bullet);
    }
    
    private void UpdateAnimationSpeed(float timeSpeed)
    {
        _animator.speed = timeSpeed;
    }
}
