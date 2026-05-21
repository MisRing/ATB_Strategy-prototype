using UnityEngine;

public class AimCamera : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private float _moveSmoothTime = 0.2f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private Vector3 _aimOffset = new Vector3(0.25f, 2f, 0f);
    [SerializeField] private float _aimDistance = 2f;
    [SerializeField] private LayerMask _visionBlockMask;
    
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private Vector3 _moveVelocity;
    private float _rotationVelocity;

    public void Update()
    {
        SmoothAim();
    }
    
    public void SetAim(Transform target, Vector3 aimTargetPosition)
    {
        if (!target) return;

        Vector3 lookDirection = (aimTargetPosition - target.position).normalized;
        Vector3 flatLookDirection = new Vector3(lookDirection.x, 0f, lookDirection.z).normalized;

        Vector3 right = Vector3.Cross(Vector3.up, flatLookDirection);

        Vector3 aimOffset = _aimOffset;

        Vector3 offset =
            -lookDirection * _aimDistance +
            right * aimOffset.x +
            Vector3.up * aimOffset.y;

        Vector3 cameraPosition = target.position + offset;

        Vector3 rayDirection = cameraPosition - aimTargetPosition;
        float rayDistance = rayDirection.magnitude;

        if (Physics.Raycast(
                aimTargetPosition,
                rayDirection.normalized,
                rayDistance,
                _visionBlockMask))
        {
            aimOffset.x = -aimOffset.x;

            offset =
                -lookDirection * _aimDistance +
                right * aimOffset.x +
                Vector3.up * aimOffset.y;

            cameraPosition = target.position + offset;
        }

        _targetPosition = cameraPosition;

        _targetRotation = Quaternion.LookRotation(aimTargetPosition - _targetPosition);
    }

    private void SmoothAim()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position,
            _targetPosition,
            ref _moveVelocity,
            _moveSmoothTime
            );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            _targetRotation,
            Time.deltaTime * _rotationSpeed
            );
    }
}
