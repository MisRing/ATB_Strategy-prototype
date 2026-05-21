using UnityEngine;

public class SprintSkill : BasicSkill
{
    private FloatStat.FloatStutBuff SpeedBuff = new FloatStat.FloatStutBuff("Spring", 0);
    public override void Init(UnitSkillController skillController)
    {
        base.Init(skillController);
        SkillName = "Sprint";
        SkillDescription = "Buff next move to x1.5 speed";
        _skillCost = 0;
        _skillCooldown = 20;
    }

    public override bool Execute(out int cost)
    {
        cost = _skillCost;

        if (!CanExecute()) return false;

        _skillCooldownTimer = _skillCooldown;

        _skillController.Unit.Stats.Speed.RemoveBuff(SpeedBuff);
        SpeedBuff = new FloatStat.FloatStutBuff("Spring", _skillController.Unit.Stats.Speed * 0.5f);
        _skillController.Unit.Stats.Speed.SetBuff(SpeedBuff);

        GameLogService.ShowMessage("Sprint!", _skillController.Unit.Combat.BodyParts.Body, 0f, 0, -1f);

        _skillController.OnSkillFinished += RemoveBuff;

        return true;
    }

    private void RemoveBuff(UnitController unit)
    {
        _skillController.Unit.Stats.Speed.RemoveBuff(SpeedBuff);
        _skillController.OnSkillFinished -= RemoveBuff;
    }
}
