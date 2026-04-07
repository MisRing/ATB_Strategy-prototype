using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerSelectionManager : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private List<UnitController> _units = new List<UnitController>();
    public UnitController SelectedUnit;

    [Header("Raycast Settings")]
    [SerializeField] private float _selectRayDistance = 100f;
    
    private PlayerController _playerController;
    
    public event Action<UnitController, UnitController> OnSelectionChanged;

    public void Init(PlayerController playerController, List<Vector3Int> positionPreset)
    {
        _playerController = playerController;
        
        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].Init(GridParameters.LevelGrid.GetTile(positionPreset[i].x, positionPreset[i].z, positionPreset[i].y));
            _units[i].SkillController.OnSkillFinished += SelectReadyUnit;
        }
    }
    
    private void OnEnable()
    {
        PlayerInputController.SwitchTarget.DefaultAction += SwitchTarget;
        PlayerInputController.PointLClicl += PointLeftClick;
    }

    private void OnDisable()
    {
        PlayerInputController.SwitchTarget.DefaultAction -= SwitchTarget;
        PlayerInputController.PointLClicl -= PointLeftClick;
    }

    private void Start()
    {
        if (_units.Count >= 1)
        {
            SelectUnit(_units[0]);
        }
        _playerController.CameraController.Init(SelectedUnit.transform);
    }

    private void PointLeftClick()
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
            int newIndex = (_units.Count + _units.IndexOf(SelectedUnit) + i) % _units.Count;
            if (_units[newIndex].State == UnitState.WaitingForOrder)
            {
                SelectUnit(_units[newIndex]);
                return;
            }
        }

        if (!leaveCurrentIfCant)
        {
            UnitController oldUnit = SelectedUnit;
            DeselectCurrentUnit();
            OnSelectionChanged?.Invoke(oldUnit, null);
        }
    }
    
    private void SelectReadyUnit(UnitController unit) => SelectUnit(unit);
    
    public void SelectUnit(UnitController unit, bool focusView = true)
    {
        if (!_units.Contains(unit)) return;
        if (unit == SelectedUnit) return;
        if (unit.Owner != UnitOwner.Player) return;

        UnitController oldUnit = SelectedUnit;
        
        DeselectCurrentUnit();
        SelectCurrentUnit(unit);
        
        OnSelectionChanged?.Invoke(oldUnit, SelectedUnit);
        if (focusView)
        {
            _playerController.CameraController.EnterFocusMode(SelectedUnit.transform);
        }
    }
    
    private void DeselectCurrentUnit()
    {
        if (SelectedUnit)
        {
            SelectedUnit.Deselect();
            SelectedUnit = null;
        }
    }
    
    private void SelectCurrentUnit(UnitController unit)
    {
        SelectedUnit = unit;
        SelectedUnit.Select();
    }
}
