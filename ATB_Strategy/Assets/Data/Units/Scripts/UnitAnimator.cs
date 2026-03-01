using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private UnitComponent _unit;

    public void Init(UnitComponent unit)
    {
        _unit = unit;
    }

    public void SetMovement(float directionX, float directionZ)
    {
        Vector3 direction = new Vector3(directionX, 0f, directionZ);
        Vector3 realDirection = transform.InverseTransformDirection(direction);

        _animator.SetFloat("MoveX", realDirection.x);
        _animator.SetFloat("MoveZ", realDirection.z);

    }
}
