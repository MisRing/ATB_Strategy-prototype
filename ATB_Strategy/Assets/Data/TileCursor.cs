using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TileCursor : MonoBehaviour
{
    private Vector3 _cursorPosition;
    private InputActions _inputActions;
    [SerializeField] private LayerMask _groundMasks;
    [SerializeField] private float _rayDistance = 100f;
    [SerializeField] private Vector3 _offset = new Vector3(0,0.1f, 0);

    [SerializeField] private GameObject _spriteObject;

    private GridMap _gridMap;

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    public void Init(GridMap gridMap)
    {
        _inputActions = new InputActions();
        _gridMap = gridMap;
        _spriteObject.SetActive(false);
    }

    private void Update()
    {
        GetPosition();
    }

    public void GetPosition()
    {
        Vector2 mousePosition = _inputActions.Player.MousePosition.ReadValue<Vector2>(); ;
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
                }
                return;
            }
        }
        UnsetTileCursor();
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
}
