using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerSelectionManager : MonoBehaviour
{
    private PlayerController _playerController;
    [SerializeField] private List<UnitController> _units = new List<UnitController>();
    public UnitController SelectedUnit { get => _selectedUnit; }
    [SerializeField] private UnitController _selectedUnit;

    [SerializeField] private float _selectRayDistance = 100f;
    
    public event Action<UnitController> OnSelectionChanged;

    public void Init(PlayerController playerController, List<Vector3Int> positionPreset)
    {
        _playerController = playerController;
        
        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].Init(GridParameters.LevelGrid.GetTile(positionPreset[i].x, positionPreset[i].z, positionPreset[i].y));
        }

        if (_units.Count >= 1)
        {
            SelectUnit(_units[0]);
        }
    }

    private void OnEnable()
    {
        PlayerInputController.SwitchTarget += SwitchTarget;
        PlayerInputController.SelectObject += SelectObject;
        TurnManager.OnUnitEnterExitQ += SelectReadyUnit;
    }

    private void OnDisable()
    {
        PlayerInputController.SwitchTarget -= SwitchTarget;
        PlayerInputController.SelectObject -= SelectObject;
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
                SelectUnit(unit, false);
            }
        }
    }
    
    private void SwitchTarget() => SwitchToFreeUnit(!PlayerInputController.IsReverseModifier, true);

    public void SwitchToFreeUnit(bool next = true, bool leaveCurrentIfCant = false)
    {
        int step = next ? 1 : -1;
        for (int i = _units.Count + step; i > 0 && i < _units.Count * 2; i+= step)
        {
            int newIndex = (_units.Count + _units.IndexOf(_selectedUnit) + i) % _units.Count;
            if (_units[newIndex].State == UnitState.WaitingForOrder)
            {
                SelectUnit(_units[newIndex]);
                return;
            }
        }

        if (!leaveCurrentIfCant)
        {
            DeselectCurrentUnit();
            OnSelectionChanged?.Invoke(null);
        }
    }
    
    private void SelectReadyUnit(UnitController unit) => SelectUnit(unit);

    private void DeselectCurrentUnit()
    {
        if (_selectedUnit)
        {
            _selectedUnit.Deselect();
            _selectedUnit = null;
        }
    }
    
    public void SelectUnit(UnitController unit, bool focusView = true)
    {
        if (!_units.Contains(unit)) return;
        if (unit == _selectedUnit) return;
        if (unit.Owner != UnitOwner.Player) return;

        DeselectCurrentUnit();

        _selectedUnit = unit;
        _selectedUnit.Select();
        
        OnSelectionChanged?.Invoke(_selectedUnit);
        if (focusView)
        {
            _playerController.CameraController.EnterFocusMode(_selectedUnit.transform);
        }
    }
}
