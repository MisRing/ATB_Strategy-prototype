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
    [SerializeField] private LineRenderer _pathRenderer;

    private GridMap _gridMap;

    public void Init(GridMap gridMap)
    {
        _gridMap = gridMap;
        _spriteObject.SetActive(false);
    }

    public bool GetPosition(Vector2 mousePosition, Transform fromTarget = null)
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
                    SetTileCursor(tileWorldPos);

                    if(fromTarget)
                    {
                        SetPathRenderer(fromTarget.position, tileWorldPos);
                    }

                    return true;
                }

                return false;
            }
        }

        UnsetPathRenderer();
        UnsetTileCursor();
        return false;
    }

    private void SetTileCursor(Vector3 tileWorldPos)
    {
        _cursorPosition = tileWorldPos;
        _spriteObject.SetActive(true);
        transform.position = tileWorldPos + _offset;
    }

    private void UnsetTileCursor()
    {
        _spriteObject.SetActive(false);
    }

    private void SetPathRenderer(Vector3 fromPoint, Vector3 toPoint)
    {
        Vector3[] path = _gridMap.GetPath(fromPoint, toPoint);

        if(path == null)
        {
            _pathRenderer.gameObject.SetActive(false);
            return;
        }

        for(int i = 0; i < path.Length; i++)
        {
            path[i] += _offset;
        }

        _pathRenderer.gameObject.SetActive(true);

        _pathRenderer.SetPositions(path);

    }

    private void UnsetPathRenderer()
    {
        _pathRenderer.gameObject.SetActive(false);
    }
}
