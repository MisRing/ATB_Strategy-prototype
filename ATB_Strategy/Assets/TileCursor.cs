using UnityEngine;

public class TileCursor : MonoBehaviour
{
    private Vector3 _cursorPosition;
    [SerializeField] private Vector3 _offset = new Vector3(0,0.1f, 0);

    public void SetPosition(Vector3 newPosition)
    {
        if (newPosition == _cursorPosition) return;

        _cursorPosition = newPosition;
        transform.position = _cursorPosition + _offset;
    }
}
