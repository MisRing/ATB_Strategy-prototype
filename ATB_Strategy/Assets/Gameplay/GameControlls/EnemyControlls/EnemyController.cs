using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private bool _doNothing = false;
    private UnitOwner _teamOwner;
    private List<UnitController> _units = new List<UnitController>();
    private UnitAIController[] _unitAIControls;

    private Dictionary<CombatObject, AITarget> _allTargets;

    private bool _squadSleeping = true;
    
    public void Init(UnitOwner owner, List<UnitController> units, UnitAIController[] unitAIs)
    {
        _teamOwner = owner;
        _units = units;
        _unitAIControls = unitAIs;
    }

    private void OnEnable()
    {
        for (int i = 0; i < _units.Count; i++)
        {
            if(!_units[i]) continue;
            _units[i].SkillController.OnSkillFinished += StartUnitAbility;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _units.Count; i++)
        {
            if(!_units[i]) continue;
            _units[i].SkillController.OnSkillFinished -= StartUnitAbility;
        }
    }

    private void Start()
    {
        SetTargets();
        for (int i = 0; i < _units.Count; i++)
        {
            StartUnitAbility(_units[i]);
        }
    }

    private void StartUnitAbility(UnitController unit)
    {
        if (!_units.Contains(unit) || unit.State != UnitState.WaitingForOrder) return;

        if (!UpdateTargets() && _squadSleeping)
        {
            TryFallback(unit);
            return;
        }

        if (_squadSleeping)
        {
            List<GridTile> unitTiles = new List<GridTile>();

            foreach (UnitController unitController in _units)
            {
                unitTiles.Add(unitController.Agent.CurrentTile);
            }
            FogOfWarUtility.ForceVisibility(unitTiles, 1f);
            
            _squadSleeping = false;
        }
        
        if (_doNothing)
        {
            TryFallback(unit);
            return;
        }
        
        int unitIndex = _units.IndexOf(unit);

        UnitAIContext context = _unitAIControls[unitIndex].GetDecision(_allTargets.Values.ToList());

        bool success = context.Decision switch
        {
            UnitAIDecision.Attack   => TryAttack(unit, context),
            UnitAIDecision.Relocate => TryRelocate(unit, context),
            _                       => false
        };
        
        if (!success)
        {
            TryFallback(unit);
            if (context.Decision != UnitAIDecision.None)
            {
                Debug.LogWarning(
                    "Fallback Error! < " + unit.name + " | Team: " + unit.Owner + " | Decision: " + context.Decision + " >"
                    );
            }
        }
    }

    private bool TryAttack(UnitController unit, UnitAIContext context)
    {
        BasicSkill skill = unit.SkillController.GetSkillByIndex(1);

        if (skill is not ITargetSwitchable attackSkill) return false;

        if (context.AttackTargetID < 0) return false;

        attackSkill.Switch(context.AttackTargetID);

        return TryExecute(unit, 1, null);
    }

    private bool TryRelocate(UnitController unit, UnitAIContext context)
    {

        PointData data = new PointData
        {
            Position = context.TargetPosition
        };

        if (TryExecute(unit, 0, data)) return true;

        string dataErrorLog =
            $"From ({unit.transform.position}), To ({context.TargetPosition})";
        Debug.LogWarning($"Relocate debug: [{unit.name} - {unit.Owner}; Path data: {dataErrorLog}");
        
        return false;
    }
    
    private bool TryExecute(UnitController unit, int skillIndex, SkillData data)
    {
        return unit.SkillController.ForceExecuteSkill(skillIndex, data, out bool isInstant);
    }
    
    private bool TryFallback(UnitController unit)
    {
        return TryExecute(unit, 3, null);
    }

    private void SetTargets()
    {
        List<CombatObject> combats = CombatManager.GetAllCombats(_teamOwner);

        _allTargets = new Dictionary<CombatObject, AITarget>();
        foreach (CombatObject combat in combats)
        {
            _allTargets.Add(combat, new AITarget(combat, combat.Position));
        }
    }

    private bool UpdateTargets()
    {
        foreach (AITarget target in _allTargets.Values)
        {
            target.IsVisible = false;
        }

        List<CombatObject> visibleCombats = GetAllVisibleCombats();

        if (visibleCombats.Count == 0) return false;
        
        foreach(CombatObject visibleTarget in visibleCombats)
        {
            if (!_allTargets.ContainsKey(visibleTarget)) continue;

            _allTargets[visibleTarget].IsVisible = true;
            _allTargets[visibleTarget].Position = visibleTarget.Position;
        }

        return true;
    }

    private List<CombatObject> GetAllVisibleCombats()
    {
        List<CombatObject> allCombats = new List<CombatObject>();

        foreach (UnitController unit in _units)
        {
            if (unit.State == UnitState.Dead) continue;

            foreach(CombatObject target in unit.Combat.Targets)
            {
                if (allCombats.Contains(target)) continue;

                allCombats.Add(target);
            }
        }

        return allCombats;
    }
}

public class AITarget
{
    public CombatObject Target;
    public bool IsVisible = false;
    public Vector3 Position;
    public int Priority = 5;

    public AITarget(CombatObject target, Vector3 position)
    {
        Target = target;
        Position = position;
    }
}
