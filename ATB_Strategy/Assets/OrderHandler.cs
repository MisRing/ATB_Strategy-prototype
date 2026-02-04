using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrderHandler : MonoBehaviour
{
    private InputActions _inputActions;
    [SerializeField] private LayerMask _groundMasks;
    [SerializeField] private TileCursor _tileCursor;

    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void Update()
    {
        Vector2 mousePosition = _inputActions.PlayerControls.MousePosition.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, _groundMasks))
        {
            Vector3 realPoint = hit.point;
            Vector3 tilePoint = new Vector3(Mathf.RoundToInt(realPoint.x), Mathf.RoundToInt(realPoint.y), Mathf.RoundToInt(realPoint.z));
            Debug.Log(realPoint);

            _tileCursor.CursorPosition = tilePoint;
        }
    }
}
