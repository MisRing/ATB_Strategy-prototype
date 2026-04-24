using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerSelectionManager : MonoBehaviour
{
    //[Header("Main Settings")]
    private List<UnitController> _units = new List<UnitController>();
    public UnitController SelectedUnit;

    [Header("Raycast Settings")]
    [SerializeField] private float _selectRayDistance = 100f;
    
    private PlayerController _playerController;
    
    public event Action<UnitController, UnitController> OnSelectionChanged;

    public void Init(PlayerController playerController, List<UnitController> units)
    {
        _playerController = playerController;
        _units = units;
        
        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].SkillController.OnSkillFinished += SelectReadyUnit;
        }
    }
    
    private void OnEnable()
    {
        PlayerInputController.SwitchTarget.DefaultAction += SwitchTarget;
        PlayerInputController.PointLClick += PointLeftClick;
    }

    private void OnDisable()
    {
        PlayerInputController.SwitchTarget.DefaultAction -= SwitchTarget;
        PlayerInputController.PointLClick -= PointLeftClick;
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
        if (RaycastExtensions.IsPointerOverUIObject()) return;

        Vector2 mousePosition = PlayerInputController.MouseScreenPosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _selectRayDistance))
        {
            UnitController unit = hit.collider.gameObject.GetComponent<UnitController>();
            if(unit != null)
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
        if (unit == SelectedUnit) return;
        if (unit.Owner != UnitOwner.Player)
        {
            TrySelectTarget(unit);
            return;
        }
        if (!_units.Contains(unit)) return;
        if(unit.State != UnitState.WaitingForOrder) return;

        UnitController oldUnit = SelectedUnit;
        
        DeselectCurrentUnit();
        SelectCurrentUnit(unit);
        
        OnSelectionChanged?.Invoke(oldUnit, SelectedUnit);
        if (focusView)
        {
            _playerController.CameraController.FocusTarget(SelectedUnit.transform);
        }
    }

    public void TrySelectTarget(UnitController unit)
    {
        if (!SelectedUnit) return;
        int index = SelectedUnit.UnitCombat.CheckTarget(unit);

        if (index != -1)
        {
            _playerController.AbilitySwitchTarget(index);
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
