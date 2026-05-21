using System;
using System.Linq;
using UnityEditor;
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

#if  UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (State == UnitState.Dead) return;

        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.up, Stats.VisionRange);

        if (Agent.PathData != null && Agent.PathData.Points != null && Agent.PathData.Points.Count >= 2)
        {
            Handles.color = Color.blue;
            Handles.DrawLine(transform.position, Agent.PathData.Points.Last());
        }

        Handles.color = Color.red;
        foreach (CombatObject target in Combat.Targets)
        {
            Handles.DrawLine(transform.position, target.Position);
        }
    }
#endif
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
