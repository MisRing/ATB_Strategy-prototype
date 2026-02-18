using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
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
    [SerializeField] private UnitComponent _selectedUnit;

    private void Awake()
    {
        _inputActions = new InputActions();

        Init();

        _tileCursor.Init(_gridMap);
        _cameraController.Init(_selectedUnit.transform);
    }

    private void Init()
    {
        _selectedUnit = _units[0];
        _selectedUnit.Select();
        for (int i = 0; i < _units.Count; i++)
        {
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

        Transform selectedTransform = null;
        if(_selectedUnit != null)
        {
            selectedTransform = _selectedUnit.transform;
        }

        _tileCursor.GetPosition(mouseScreenPosition, selectedTransform);
    }

    private void SwitchTarget(InputAction.CallbackContext context)
    {
        int newIndex = 0;
        if(_inputActions.Player.ReverseInputModifier.IsPressed())
        {
            newIndex = (_units.IndexOf(_selectedUnit) - 1 + _units.Count) % _units.Count;
        }
        else
        {
            newIndex = (_units.IndexOf(_selectedUnit) + 1) % _units.Count;
        }

        _selectedUnit.Deselect();
        _selectedUnit = _units[newIndex];
        _selectedUnit.Select();
        _cameraController.EnterFocusMode(_selectedUnit.transform);
    }
}
