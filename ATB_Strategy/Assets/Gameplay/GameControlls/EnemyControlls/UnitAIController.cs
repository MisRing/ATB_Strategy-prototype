using UnityEngine;

public class UnitAIController : MonoBehaviour
{

    private UnitController _unit;
    
    public void Init()
    {
        _unit = GetComponent<UnitController>();
    }

    public UnitAIContext GetDecision()
    {
        int attackWeight = 20;
        int relocateWeight = 20;

        foreach (CombatTarget target in _unit.Combat.Targets)
        {
            Vector3Int currentTilePos = _unit.Agent.CurrentTile;
            GridTile currentTile = GridParameters.LevelGrid.GetTile(currentTilePos.x, currentTilePos.z, currentTilePos.y);
            int defenceFromTarget = CombatService.CalculateCoverDodge(
                currentTile,
                _unit.Stats.Dodge,
                target.Target.Position
                );

            if (defenceFromTarget <= _unit.Stats.Dodge)
            {
                relocateWeight += 100;
                continue;
            }
            if (defenceFromTarget >= 80)
            {
                relocateWeight += 10;
                continue;
            }
            if (defenceFromTarget >= 50)
            {
                relocateWeight += 50;
                continue;
            }
        }
        
        int targetID = -1;
        int hitWeight = -100;
        
        if (_unit.Combat.Targets.Count == 0)
        {
            attackWeight -= 100;
        }
        else
        {
            BasicSkill skill = _unit.SkillController.GetSkillByIndex(1);

            if (skill is ITargetSwitchable)
            {
                ITargetSwitchable attackSkill = skill as ITargetSwitchable;
                for (int i = 0; i < _unit.Combat.Targets.Count; i++)
                {
                    attackSkill.Switch(i);
                    int currentHitWeight = attackSkill.SelectedTargetContext.HitChance +
                                           attackSkill.SelectedTargetContext.CritChance * 4;
                    if (currentHitWeight > hitWeight)
                    {
                        hitWeight = currentHitWeight;
                        targetID = i;
                    }
                }

                if (hitWeight > 100)
                {
                    attackWeight += 100;
                }
                else if (hitWeight > 80)
                {
                    attackWeight += 60;
                }
                else if (hitWeight > 60)
                {
                    attackWeight += 20;
                }
                else
                {
                    attackWeight -= 40;
                }
            }
            else
            {
                attackWeight -= 100;
            }
        }

        UnitAIContext aiContext = new UnitAIContext();
        aiContext.AttackTargetID = targetID;

        if (relocateWeight <= 0 && attackWeight <= 0)
        {
            aiContext.Decision = UnitAIDecision.None;
        }
        else if (attackWeight > relocateWeight && targetID != -1)
        {
            aiContext.Decision = UnitAIDecision.Attack;
        }
        else
        {
            aiContext.Decision = UnitAIDecision.Relocate;
        }

        return aiContext;
    }
}

public struct UnitAIContext
{
    public UnitAIDecision Decision;
    public Vector3 TargetPosition;
    public int AttackTargetID;
}

public enum UnitAIDecision
{
    None,
    Attack,
    Relocate,
}
