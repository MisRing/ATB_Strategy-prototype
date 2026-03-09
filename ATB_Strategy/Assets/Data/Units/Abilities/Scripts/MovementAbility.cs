using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MovementAbility : AbilityBasic, IPathHandler
{
    private PathData _pathData;
        
    public event Action<PathData> OnPathChanged;

    private void Awake()
    {
        AbilityViewRenderer.PathHandlers.Add(this);
    }

    public override void Init(UnitAbilityController abilityController)
    {
        base.Init(abilityController);
        AbilityName = "Simple movement";
    }

    public override void EnterPrepare(AbilityData abilityData)
    {
        base.EnterPrepare(abilityData);

        UpdateData(abilityData);
    }

    public override void ExitPrepare()
    {
        base.ExitPrepare();
    }

    public override void UpdateData(AbilityData abilityData)
    {
        if (_abilityController.Unit.State != UnitState.WaitingForOrder) return;

        base.UpdateData(abilityData);
        _pathData.IsReacheble = _abilityController.Unit.AgentController.CalculatePath(ref _pathData, _abilityData.TargetWorldPos);

        OnPathChanged?.Invoke(_pathData);
    }

    public override bool Execute()
    {
        if (!_pathData.IsReacheble) return false;

        _abilityController.Unit.AgentController.OnMoveComplete += FinishExecute;
        _abilityController.Unit.AgentController.StartMove(_pathData);

        PathData emptyPath = new PathData();
        emptyPath.IsReacheble = false;
        OnPathChanged?.Invoke(emptyPath);

        return base.Execute();
    }

    private void FinishExecute()
    {
        _abilityController.Unit.AgentController.OnMoveComplete -= FinishExecute;
        _abilityController.FinishExecuteAbility();
    }
}
