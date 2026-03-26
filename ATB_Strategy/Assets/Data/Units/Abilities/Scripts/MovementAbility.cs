using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MovementAbility : AbilityBasic, IPathHandler
{
    private PathData _pathData { get { return _abilityController.Unit.AgentController.PathData; } }
        
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

    public override void EnterPrepare()
    {
        base.EnterPrepare();
    }

    public override void ExitPrepare()
    {
        base.ExitPrepare();
    }

    public override void UpdateData(AbilityData abilityData)
    {
        if (_abilityController.Unit.State != UnitState.WaitingForOrder) return;

        base.UpdateData(abilityData);
        _abilityController.Unit.AgentController.CalculatePath(_abilityData.TargetWorldPos);

        OnPathChanged?.Invoke(_pathData);
    }

    public override bool Execute()
    {
        if (!_pathData.IsReacheble) return false;

        //_abilityController.Unit.AgentController.OnMoveComplete += FinishExecute;
        _abilityController.Unit.AgentController.StartMove();

        PathData emptyPath = new PathData();
        emptyPath.IsReacheble = false;
        OnPathChanged?.Invoke(emptyPath);
        
        Debug.Log("Start executing <" + AbilityName + "> | Cost: " + _pathData.TurnsCost);

        TurnManager.EnterBusyQ(this, _pathData.TurnsCost);

        return true;
    }

    public override void FinishExecute()
    {
        //_abilityController.Unit.AgentController.OnMoveComplete -= FinishExecute;
        //_abilityController.FinishExecuteAbility();
        base.FinishExecute();
    }
}
