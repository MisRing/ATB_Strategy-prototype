using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TileCursor : MonoBehaviour
{
    private Vector3 _cursorPosition;
    [SerializeField] private LayerMask _groundMasks;
    [SerializeField] private float _rayDistance = 100f;
    [SerializeField] private Vector3 _offset = new Vector3(0,0.1f, 0);

    [SerializeField] private GameObject _spriteObject;

    private GridMap _gridMap;

    public void Init(GridMap gridMap)
    {
        _gridMap = gridMap;
        _spriteObject.SetActive(false);
    }

    public bool GetPosition(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _rayDistance, _groundMasks))
        {
            Vector3 realPoint = hit.point;
            
            if(_gridMap.HasTile(realPoint.x, realPoint.z))
            {
                Vector3 tileWorldPos = _gridMap.GetTileWorldPosition(realPoint.x, realPoint.z);
                if (_cursorPosition != tileWorldPos)
                {
                    _cursorPosition = tileWorldPos;
                    _spriteObject.SetActive(true);
                    transform.position = tileWorldPos + _offset;
                    return true;
                }

                return false;
            }
        }

        _spriteObject.SetActive(false);
        return false;
    }
}
