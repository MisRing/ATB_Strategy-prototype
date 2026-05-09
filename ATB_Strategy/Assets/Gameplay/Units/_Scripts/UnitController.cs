using System;
using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitSkillController))]
[RequireComponent(typeof(UnitCombat))]
[RequireComponent(typeof(UnitAnimator))]
[RequireComponent(typeof(UnitPreviewAnimator))]
[RequireComponent(typeof(UnitAgent))]
public class UnitController : MonoBehaviour
{
    [HideInInspector] public UnitStats Stats;
    [HideInInspector] public UnitSkillController SkillController;
    [HideInInspector] public UnitCombat Combat;
    [HideInInspector] public UnitAnimator Animator;
    [HideInInspector] public UnitPreviewAnimator PreviewAnimator;
    [HideInInspector] public UnitAgent Agent;

    public event Action<bool> OnSelectionChanged;

    private bool _isSelected;

    public UnitState State;
    public UnitOwner Owner;

    public void Init(GridTile tile)
    {        
        Stats = GetComponent<UnitStats>();
        SkillController = GetComponent<UnitSkillController>();
        Combat = GetComponent<UnitCombat>();
        Animator = GetComponent<UnitAnimator>();
        PreviewAnimator = GetComponent<UnitPreviewAnimator>();
        Agent = GetComponent<UnitAgent>();

        SkillController.Init(this);
        Combat.Init(this);
        Animator.Init(this);
        Agent.Init(this, tile);
        
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
    Engaged,
    Dead
}

public enum UnitOwner
{
    None,
    PlayerTeam,
    EnemyTeam0,
    EnemyTeam1,
    EnemyTeam2,
}
