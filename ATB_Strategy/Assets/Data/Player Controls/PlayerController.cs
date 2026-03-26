using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    private CursorController _cursorController;

    [SerializeField] private List<UnitController> _units = new List<UnitController>();
    [SerializeField] private List<Vector3Int> _positionPreset = new List<Vector3Int>();
    public UnitController SelectedUnit {get => _selectedUnit; }
    [SerializeField] private UnitController _selectedUnit;

    [SerializeField] private float _selectRayDistance = 100f;
    
    public event Action<UnitController> OnSelectionChanged;

    private void Awake()
    {
        _cursorController = GetComponent<CursorController>();

        Init();

        _cursorController.Init();
        _cameraController.Init(_selectedUnit.transform);
    }

    private void Init()
    {
        if (GridParameters.LevelGrid == null)
        {
            GridParameters.LevelGrid = FindFirstObjectByType(typeof(GridMap)) as GridMap;
        }

        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].Init(GridParameters.LevelGrid.GetTile(_positionPreset[i].x, _positionPreset[i].z, _positionPreset[i].y));
        }
        SelectTargetUnit(_units[0]);
    }

    private void OnEnable()
    {
        PlayerInputController.SwitchTarget += SwitchTarget;
        PlayerInputController.SelectObject += SelectObject;
        PlayerInputController.SelectPoint += SelectPoint;
        PlayerInputController.SelectAbility += SelectAbility;

        _cursorController.OnPositionChanged += UpdateAbilityData;

        TurnManager.OnUnitEnterExitQ += SelectReadyUnit;
    }

    private void OnDisable()
    {
        PlayerInputController.SwitchTarget -= SwitchTarget;
        PlayerInputController.SelectObject -= SelectObject;
        PlayerInputController.SelectPoint -= SelectPoint;
        PlayerInputController.SelectAbility -= SelectAbility;

        _cursorController.OnPositionChanged -= UpdateAbilityData;

        TurnManager.OnUnitEnterExitQ -= SelectReadyUnit;
    }

    private void SelectObject()
    {
        Vector2 mousePosition = PlayerInputController.MouseScreenPosition;
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
        data.TargetTile = _cursorController.CursorTile;

        if (_selectedUnit.AbilityController.ExecuteAbility(data))
        {
            if(!SwitchToFreeUnit(+1))
            {
                DeselectCurrentUnit();
                OnSelectionChanged?.Invoke(null);
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
        if (!PlayerInputController.IsReverseModifier)
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
        if (unit.Owner != UnitOwner.Player) return;

        DeselectCurrentUnit();

        _selectedUnit = unit;
        AbilityData data = new AbilityData();
        data.TargetWorldPos = _cursorController.CursorPosition;
        _selectedUnit.Select(data);
        
        OnSelectionChanged?.Invoke(_selectedUnit);
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

    private void OnDrawGizmos()
    {
        if(GridParameters.LevelGrid == null)
        {
            GridParameters.LevelGrid = FindFirstObjectByType(typeof(GridMap)) as GridMap;
        }

        foreach(Vector3Int point in _positionPreset)
        {
            if(GridParameters.LevelGrid.CheckTile(point.x, point.z, point.y))
            {
                Gizmos.color = Color.darkGreen;
                Gizmos.DrawSphere(GridParameters.LevelGrid.GetTileWorldPos(point.x, point.z, point.y), 0.3f);
            }
        }
    }
}