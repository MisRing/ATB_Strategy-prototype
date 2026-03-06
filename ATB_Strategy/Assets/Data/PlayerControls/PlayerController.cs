using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    private CursorController _cursorController;
    private PlayerInputController _playerInputController;

    [SerializeField] private List<UnitController> _units = new List<UnitController>();
    [SerializeField] private List<Vector2Int> _positionPresset = new List<Vector2Int>();
    [SerializeField] private UnitController _selectedUnit;

    [SerializeField] private float _selectRayDistance = 100f;

    private void Awake()
    {
        _playerInputController = GetComponent<PlayerInputController>();
        _cursorController = GetComponent<CursorController>();
    }

    private void Start()
    {
        Init();

        _cursorController.Init(_playerInputController);
        _cameraController.Init(_selectedUnit.transform);
    }

    private void Init()
    {
        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].Init(GridParameters.LevelGrid.GetTile(_positionPresset[i].x, _positionPresset[i].y));
        }
        SelectTargetUnit(_units[0]);
    }

    private void OnEnable()
    {
        _playerInputController.SwitchTarget += SwitchTarget;
        _playerInputController.SelectObject += SelectObject;
        _playerInputController.SelectPoint += SelectPoint;
        _playerInputController.SelectAbility += SelectAbility;

        _cursorController.OnPositionChanged += UpdateAbilityData;

        TurnManager.OnUnitEnterExitQ += SelectReadyUnit;
    }

    private void OnDisable()
    {
        _playerInputController.SwitchTarget -= SwitchTarget;
        _playerInputController.SelectObject -= SelectObject;
        _playerInputController.SelectPoint -= SelectPoint;
        _playerInputController.SelectAbility -= SelectAbility;

        _cursorController.OnPositionChanged -= UpdateAbilityData;

        TurnManager.OnUnitEnterExitQ -= SelectReadyUnit;
    }

    private void SelectObject()
    {
        Vector2 mousePosition = _playerInputController.MouseScreenPosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _selectRayDistance))
        {
            UnitController unit = hit.collider.gameObject.GetComponent<UnitController>();
            if(unit != null && unit.State == UnitState.WaitingForOrder)
            {
                SelectTargetUnit(unit, false);
            }
        }
    }

    private void SelectPoint()
    {
        if (!_selectedUnit || _selectedUnit.State == UnitState.Engaged) return;

        AbilityData data = new AbilityData();
        data.TargetWorldPos = _cursorController.CursorPosition;
        if(_selectedUnit.AbilityController.ExecuteAbility(data))
        {
            if(!SwitchToFreeUnit(+1))
            {
                DeselectCurrentUnit();
            }
        }
    }

    private void UpdateAbilityData()
    {
        if (!_selectedUnit || _selectedUnit.State == UnitState.Engaged) return;

        AbilityData data = new AbilityData();
        data.TargetWorldPos = _cursorController.CursorPosition;
        _selectedUnit.AbilityController.UpdateAbilityData(data);
    }

    private void SwitchTarget()
    {
        int findStep;
        if (!_playerInputController.IsReverseModifier)
        {
            findStep = 1;
        }
        else
        {
            findStep = -1;
        }

        SwitchToFreeUnit(findStep);
    }

    private bool SwitchToFreeUnit(int step)
    {
        for (int i = _units.Count + step; i > 0 && i < _units.Count * 2; i+= step)
        {
            int newIndex = (_units.Count + _units.IndexOf(_selectedUnit) + i) % _units.Count;
            if (_units[newIndex].State == UnitState.WaitingForOrder)
            {
                SelectTargetUnit(_units[newIndex]);
                return true;
            }
        }

        return false;
    }

    private void SelectReadyUnit(UnitController unit)
    {
        if (!_units.Contains(unit)) return;
        if (_selectedUnit) return;

        SelectTargetUnit(unit);
    }

    private void SelectTargetUnit(UnitController unit, bool focusView = true)
    {
        if (unit == _selectedUnit) return;

        DeselectCurrentUnit();

        _selectedUnit = unit;
        AbilityData data = new AbilityData();
        data.TargetWorldPos = _cursorController.CursorPosition;
        _selectedUnit.Select(data);
        if (focusView)
        {
            _cameraController.EnterFocusMode(_selectedUnit.transform);
        }
    }

    private void DeselectCurrentUnit()
    {
        if (_selectedUnit)
        {
            _selectedUnit.Deselect();
            _selectedUnit = null;
        }
    }

    private void SelectAbility(int index)
    {
        if (!_selectedUnit) return;

        AbilityData data = new AbilityData();
        data.TargetWorldPos = _cursorController.CursorPosition;
        _selectedUnit.AbilityController.SelectAbility(index, data);
    }
}