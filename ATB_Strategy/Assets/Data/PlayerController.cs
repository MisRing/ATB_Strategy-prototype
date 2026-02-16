using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputActions _inputActions;
    [SerializeField] private GridMap _gridMap;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private TileCursor _tileCursor;

    [SerializeField] private List<UnitComponent> _units = new List<UnitComponent>();
    [SerializeField] private List<Vector2Int> _positionPresset = new List<Vector2Int>();
    [SerializeField] private Queue<UnitComponent> _unitQ = new Queue<UnitComponent>();

    private void Awake()
    {
        _inputActions = new InputActions();

        Init();

        _tileCursor.Init(_gridMap);
        _cameraController.Init(_unitQ.Peek().transform.position);
    }

    private void Init()
    {
        _unitQ.Clear();

        for(int i = 0; i < _units.Count; i++)
        {
            _unitQ.Enqueue(_units[i]);
            _units[i].Init(_gridMap._grid[GridMapExtansion.GetIndex(_gridMap, _positionPresset[i].x, _positionPresset[i].y)]);
        }
    }

    private void OnEnable()
    {
        _inputActions.Enable();

        _inputActions.Player.SwitchTarget.started += SwitchTarget;
    }

    private void OnDisable()
    {
        _inputActions.Disable();

        _inputActions.Player.SwitchTarget.started += SwitchTarget;
    }

    private void Update()
    {
        Vector2 mouseScreenPosition = _inputActions.Player.MousePosition.ReadValue<Vector2>();
        _tileCursor.GetPosition(mouseScreenPosition);
    }

    private void SwitchTarget(InputAction.CallbackContext context)
    {
        if(_inputActions.Player.ReverseInputModifier.IsPressed())
        {
            Debug.Log("PREV TARGET");
        }
        else
        {
            Debug.Log("NEXT TARGET");
        }
    }
}
