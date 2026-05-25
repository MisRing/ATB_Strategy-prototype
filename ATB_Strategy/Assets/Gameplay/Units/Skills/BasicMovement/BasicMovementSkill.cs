using UnityEngine;

public class BasicMovementSkill : BasicSkill
{
    private UnitAgent _agent;
    public override void Init(UnitSkillController skillController)
    {
        base.Init(skillController);
        //SkillName = "Basic movement";
        RequiredDataType = typeof(PointData);
        _agent = _skillController.Unit.Agent;
        _skillCooldown = 0;
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

    public override bool Execute(out int cost, ref UnitState state)
    {
        cost = 0;
        if (!CanExecute()) return false;
        if (!_agent.StartMove(out cost)) return false;
        state = UnitState.Moving;
        return true;
    }
}
