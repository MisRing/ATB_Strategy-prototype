using System;
using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitAbilityController))]
[RequireComponent(typeof(UnitAnimator))]
public class UnitController : MonoBehaviour
{
    [HideInInspector] public UnitStats UnitStats;
    [HideInInspector] public UnitAbilityController AbilityController;
    [HideInInspector] public UnitAnimator UnitAnimator;
    [HideInInspector] public GridMap GridMap;

    public event Action<bool> OnSelectionChanged;

    private bool _isSelected;

    public UnitState State;

    public void Init(GridTile tile, GridMap gridMap)
    {        
        UnitStats = GetComponent<UnitStats>();
        AbilityController = GetComponent<UnitAbilityController>();
        UnitAnimator = GetComponent<UnitAnimator>();
        GridMap = gridMap;

        transform.position = new Vector3(tile.PositionX, tile.DeltaY, tile.PositionZ) + tile.GridOffset;

        AbilityController.Init(this);
        UnitAnimator.Init(this);
    }

    public void Select(AbilityData data)
    {
        if (_isSelected) return;

        _isSelected = true;
        OnSelectionChanged?.Invoke(true);

        AbilityController.SelectAbility(0, data);
    }

    public void Deselect()
    {
        if (!_isSelected) return;

        _isSelected = false;
        OnSelectionChanged?.Invoke(false);

        if (State != UnitState.Engaged)
        {
            AbilityController.DeselectAbility();
        }
    }
}

public enum UnitState
{
    WaitingForOrder,
    Engaged
}
