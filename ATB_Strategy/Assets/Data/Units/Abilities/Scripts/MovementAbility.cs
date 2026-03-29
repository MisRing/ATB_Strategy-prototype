using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MovementAbility : AbilityBasic
{
    private UnitAgent _agent;
    public override void Init(UnitAbilityController abilityController)
    {
        base.Init(abilityController);
        AbilityName = "Simple movement";
        _agent = Unit.AgentController;
    }

    public override void EnterPrepare()
    {
        _agent.ShowPath(true);
        base.EnterPrepare();
    }

    public override void ExitPrepare()
    {
        _agent.ShowPath(false);
        base.ExitPrepare();
    }

    public override void UpdateData(AbilityData abilityData)
    {
        if (_abilityController.Unit.State != UnitState.WaitingForOrder) return;

        base.UpdateData(abilityData);
        _agent.CalculatePath(_abilityData.TargetWorldPos);
    }

    public override bool Execute()
    {
        if(!_agent.StartMove()) return false;
        
        Debug.Log("Start executing <" + AbilityName + "> | Cost: " + _agent.PathData.TurnsCost);

        TurnManager.EnterBusyQ(this, _agent.PathData.TurnsCost);

        return true;
    }

    public override void FinishExecute()
    {
        base.FinishExecute();
    }
}
