using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitSkillController))]
[RequireComponent(typeof(UnitAnimator))]
[RequireComponent(typeof(UnitAgent))]
public class UnitController : MonoBehaviour
{
    [HideInInspector] public UnitStats UnitStats;
    [HideInInspector] public UnitSkillController SkillController;
    [HideInInspector] public UnitAnimator UnitAnimator;
    [HideInInspector] public UnitAgent AgentController;

    public event Action<bool> OnSelectionChanged;

    private bool _isSelected;

    public UnitState State;
    public UnitOwner Owner;

    public void Init(GridTile tile)
    {        
        UnitStats = GetComponent<UnitStats>();
        SkillController = GetComponent<UnitSkillController>();
        UnitAnimator = GetComponent<UnitAnimator>();
        AgentController = GetComponent<UnitAgent>();

        SkillController.Init(this);
        UnitAnimator.Init(this);
        AgentController.Init(this, tile);
        
        TurnManager.EnterWaitingQ(this);
    }

    public void Select()
    {
        if (_isSelected) return;

        _isSelected = true;
        OnSelectionChanged?.Invoke(_isSelected);
    }

    public void Deselect()
    {
        if (!_isSelected) return;

        _isSelected = false;
        OnSelectionChanged?.Invoke(_isSelected);
    }
}

public enum UnitState
{
    WaitingForOrder,
    Engaged
}

public enum UnitOwner
{
    Player,
    Enemy
}
