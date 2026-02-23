using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GridMap _gridMap;
    [SerializeField] private CameraController _cameraController;
    private CursorController _cursorController;
    private PlayerInputController _playerInputController;

    [SerializeField] private List<UnitComponent> _units = new List<UnitComponent>();
    [SerializeField] private List<Vector2Int> _positionPresset = new List<Vector2Int>();
    [SerializeField] private UnitComponent _selectedUnit;

    [SerializeField] private float _selectRayDistance = 100f;

    private void Awake()
    {
        Init();

        _cursorController.Init(_gridMap, _playerInputController);
        _cameraController.Init(_selectedUnit.transform);
    }

    private void Init()
    {
        _playerInputController = GetComponent<PlayerInputController>();
        _cursorController = GetComponent<CursorController>();

        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].Init(_gridMap._grid[GridMapExtansion.GetIndex(_gridMap, _positionPresset[i].x, _positionPresset[i].y)]);
        }
        _selectedUnit = _units[0];
        _selectedUnit.Select();
    }

    private void OnEnable()
    {
        _playerInputController.SwitchTarget += SwitchTarget;
        _playerInputController.SelectObject += SelectObject;
        _playerInputController.SelectAbility += SelectAbility;
    }

    private void OnDisable()
    {
        _playerInputController.SwitchTarget -= SwitchTarget;
        _playerInputController.SelectObject -= SelectObject;
        _playerInputController.SelectAbility -= SelectAbility;
    }

    private void SelectObject()
    {
        Vector2 mousePosition = _playerInputController.MouseScreenPosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _selectRayDistance))
        {
            UnitComponent unit = hit.collider.gameObject.GetComponent<UnitComponent>();
            if(unit != null)
            {
                SelectTargetUnit(unit, false);
            }
        }
    }

    private void SwitchTarget()
    {
        int newIndex = 0;
        if(_playerInputController.IsReverseModifier)
        {
            newIndex = (_units.IndexOf(_selectedUnit) - 1 + _units.Count) % _units.Count;
        }
        else
        {
            newIndex = (_units.IndexOf(_selectedUnit) + 1) % _units.Count;
        }

        SelectTargetUnit(_units[newIndex]);
    }

    private void SelectTargetUnit(UnitComponent unit, bool focusView = true)
    {
        if (unit == _selectedUnit) return;

        _selectedUnit.Deselect();
        _selectedUnit = unit;
        _selectedUnit.Select();
        if (focusView)
        {
            _cameraController.EnterFocusMode(_selectedUnit.transform);
        }
    }

    private void SelectAbility(int index)
    {
        if (!_selectedUnit) return;

        _selectedUnit.AbilityController.SelectAbility(index);
    }
}