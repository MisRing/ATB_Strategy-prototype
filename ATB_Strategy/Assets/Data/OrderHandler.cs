using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrderHandler : MonoBehaviour
{
    //private InputActions _inputActions;
    //[SerializeField] private LayerMask _groundMasks;
    //[SerializeField] private TileCursor _tileCursor;
    //[SerializeField] private GridMap _gridMap;

    ////public int

    //public UnitComponent Unit;

    //private void Awake()
    //{
    //    _inputActions = new InputActions();
    //}

    //private void OnEnable()
    //{
    //    _inputActions.Enable();

    //    _inputActions.PlayerControls.MouseClick.started += MoveToMouse;
    //}

    //private void OnDisable()
    //{
    //    _inputActions.Disable();

    //    _inputActions.PlayerControls.MouseClick.started -= MoveToMouse;
    //}

    //private void Update()
    //{
    //    SelectTile();
    //}

    //private bool SelectTile()
    //{
    //    Vector2 mousePosition = _inputActions.PlayerControls.MousePosition.ReadValue<Vector2>();

    //    Ray ray = Camera.main.ScreenPointToRay(mousePosition);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, 1000f, _groundMasks))
    //    {
    //        Vector3 realPoint = hit.point;
    //        Vector3 tilePoint = _gridMap.GetTilePos(realPoint.x, realPoint.z);

    //        if (tilePoint == -Vector3.one) return false;

    //        _tileCursor.SetPosition(tilePoint);
    //        return true;
    //    }
    //    return false;
    //}

    //private void MoveToMouse(InputAction.CallbackContext context)
    //{
    //    if (!SelectTile()) return;

    //    GridTile tile = new GridTile();
    //    _gridMap.GetTileRef(ref tile, _tileCursor.GetCursorPosition().x, _tileCursor.GetCursorPosition().z);

    //    Unit.MoveToTile(ref tile, _gridMap);
    //}
}
