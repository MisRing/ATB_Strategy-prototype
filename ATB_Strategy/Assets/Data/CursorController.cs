using UnityEngine;

public class CursorController : MonoBehaviour
{
    [Header("Main settings")]
    [SerializeField] private TileCursor _tileCursor;
    [SerializeField] private PathLineRenderer _pathRenderer;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask _groundMasks;
    [SerializeField] private float _rayDistance = 100f;

    private InputActions _inputActions;
    private GridMap _gridMap;

    private Vector3 _cursorPosition;
    public Vector3 CursorPosition { get => _cursorPosition; }

    private bool _showTileCursor = true;
    public bool ShowTileCursor
    {
        get => _showTileCursor;
        set
        {
            _showTileCursor = value;
            CalculatePosition();
        }
    }

    private bool _showPathLine = false;
    public bool ShowPathLine
    {
        get => _showPathLine;
        set
        {
            _showPathLine = value;
            CalculatePosition();
        }
    }

    private bool _enableTileCursor = false;
    private bool _enablePathLine = false;


    private void Awake()
    {
        _inputActions = new InputActions();
    }

    public void Init(GridMap gridMap)
    {
        _gridMap = gridMap;
        _tileCursor.Init();
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
        CalculatePosition();
    }

    private void CalculatePosition()
    {
        Vector2 mousePosition = _inputActions.Player.MousePosition.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        Vector3 tileWorldPos = Vector3.zero;

        bool cursorOnTile = false;

        if (Physics.Raycast(ray, out hit, _rayDistance, _groundMasks))
        {
            Vector3 realPoint = hit.point;

            if (_gridMap.HasTile(realPoint.x, realPoint.z))
            {
                tileWorldPos = _gridMap.GetTileWorldPosition(realPoint.x, realPoint.z);
                cursorOnTile = true;
            }
        }

        if(cursorOnTile)
        {
            UpdateCursorPosition(tileWorldPos);
        }
        else
        {
            DisableAll();
        }
    }

    private void UpdateCursorPosition(Vector3 tileWorldPos)
    {
        if (_cursorPosition != tileWorldPos)
        {
            if(ShowTileCursor)
            {
                _tileCursor.SetTileCursor(tileWorldPos);
            }

            if (ShowPathLine)
            {
                
            }
        }
    }

    private void DisableAll()
    {
        if(_enableTileCursor)
        {
            _tileCursor.UnsetTileCursor();
        }

        if (_enablePathLine)
        {

        }

        _enableTileCursor = false;
        _enablePathLine = false;
    }
}
