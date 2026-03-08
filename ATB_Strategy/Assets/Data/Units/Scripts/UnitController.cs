using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitAbilityController))]
[RequireComponent(typeof(UnitAnimator))]
[RequireComponent(typeof(NavMeshAgent))]
public class UnitController : MonoBehaviour
{
    [HideInInspector] public UnitStats UnitStats;
    [HideInInspector] public UnitAbilityController AbilityController;
    [HideInInspector] public UnitAnimator UnitAnimator;
    [HideInInspector] public NavMeshAgent UnitAgent;

    public event Action<bool> OnSelectionChanged;

    private bool _isSelected;

    public UnitState State;

    public void Init(GridTile tile)
    {        
        UnitStats = GetComponent<UnitStats>();
        AbilityController = GetComponent<UnitAbilityController>();
        UnitAnimator = GetComponent<UnitAnimator>();
        UnitAgent = GetComponent<NavMeshAgent>();

        transform.position = GridParameters.LevelGrid.GetTileWorldPos(tile);
        UnitAgent.enabled = true;
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
