using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitAbilityController))]
[RequireComponent(typeof(UnitAnimator))]
[RequireComponent(typeof(UnitAgentController))]
public class UnitController : MonoBehaviour
{
    [HideInInspector] public UnitStats UnitStats;
    [HideInInspector] public UnitAbilityController AbilityController;
    [HideInInspector] public UnitAnimator UnitAnimator;
    [HideInInspector] public UnitAgentController AgentController;

    public event Action<bool> OnSelectionChanged;

    private bool _isSelected;

    public UnitState State;

    public void Init(GridTile tile)
    {        
        UnitStats = GetComponent<UnitStats>();
        AbilityController = GetComponent<UnitAbilityController>();
        UnitAnimator = GetComponent<UnitAnimator>();
        AgentController = GetComponent<UnitAgentController>();

        AbilityController.Init(this);
        UnitAnimator.Init(this);
        AgentController.Init(this, tile);
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
