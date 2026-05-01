using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerSelectionManager : MonoBehaviour
{
    [HideInInspector] public List<UnitController> Units = new List<UnitController>();
    [HideInInspector] public UnitController SelectedUnit;

    [Header("Raycast Settings")]
    [SerializeField] private float _selectRayDistance = 100f;
    
    private PlayerController _playerController;
    
    public event Action<UnitController, UnitController> OnSelectionChanged;

    public void Init(PlayerController playerController, List<UnitController> units)
    {
        _playerController = playerController;
        Units = units;
    }
    
    private void OnEnable()
    {
        PlayerInputController.SwitchTarget.DefaultAction += SwitchTarget;
        PlayerInputController.PointLClick += PointLeftClick;
        
        for (int i = 0; i < Units.Count; i++)
        {
            if(!Units[i]) continue;
            Units[i].SkillController.OnSkillFinished += SelectReadyUnit;
        }
    }

    private void OnDisable()
    {
        PlayerInputController.SwitchTarget.DefaultAction -= SwitchTarget;
        PlayerInputController.PointLClick -= PointLeftClick;
        
        for (int i = 0; i < Units.Count; i++)
        {
            if(!Units[i]) continue;
            Units[i].SkillController.OnSkillFinished -= SelectReadyUnit;
        }
    }

    private void Start()
    {
        if(Units.Count == 0) return;
        SelectUnit(Units[0]);
        _playerController.CameraController.FocusTarget(SelectedUnit.transform, true);
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
        for (int i = Units.Count + step; i > 0 && i < Units.Count * 2; i+= step)
        {
            int newIndex = (Units.Count + Units.IndexOf(SelectedUnit) + i) % Units.Count;
            if (Units[newIndex].State == UnitState.WaitingForOrder)
            {
                SelectUnit(Units[newIndex]);
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
        if (unit.Owner != UnitOwner.PlayerTeam)
        {
            TrySelectTarget(unit);
            return;
        }
        if (!Units.Contains(unit)) return;
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
        int index = SelectedUnit.Combat.CheckTarget(unit);

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
