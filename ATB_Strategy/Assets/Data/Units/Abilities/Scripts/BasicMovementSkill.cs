using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BasicMovementSkill : BasicSkill
{
    private UnitAgent _agent;
    public override void Init(UnitSkillController skillController)
    {
        base.Init(skillController);
        SkillName = "Basic movement";
        RequiredDataType = typeof(PointData);
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

    public override void UpdateData(SkillData skillData)
    {
        if (_skillController.Unit.State != UnitState.WaitingForOrder) return;

        base.UpdateData(skillData);
        _agent.CalculatePath((_skillData as PointData).Position);
    }

    public override bool Execute()
    {
        if(!_agent.StartMove()) return false;
        
        Debug.Log("Start executing <" + SkillName + "> | Cost: " + _agent.PathData.TurnsCost);

        TurnManager.EnterBusyQ(Unit, _agent.PathData.TurnsCost);

        return true;
    }
}
