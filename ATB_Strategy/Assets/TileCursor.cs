using UnityEngine;

public class TileCursor : MonoBehaviour
{
    public Vector3 CursorPosition;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private Vector3 _offset = new Vector3(0,0.1f, 0);

    private void Update()
    {
        transform.position = Vector3.Slerp(transform.position, CursorPosition + _offset, Time.deltaTime * _moveSpeed);
    }
}
