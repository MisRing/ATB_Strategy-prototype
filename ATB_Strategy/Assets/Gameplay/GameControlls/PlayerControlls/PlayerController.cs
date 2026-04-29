using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public CameraController CameraController;
    [HideInInspector] public CursorController CursorController;
    [HideInInspector] public PlayerSelectionManager PlayerSelectionManager;
    
    public event Action<int, bool> OnAbilitySelected;
    public event Action<int> OnTargetSwitched;
    public event Action OnAbilityExecuted;

    public void Init(List<UnitController> units)
    {
        CursorController = GetComponent<CursorController>();
        PlayerSelectionManager = GetComponent<PlayerSelectionManager>();
        
        PlayerSelectionManager.Init(this, units);

        CursorController.Init();
    }

    private void OnEnable()
    {
        PlayerInputController.SelectAbility += SelectAbility;

        PlayerSelectionManager.OnSelectionChanged += UnitSelected;

        GameGlobalComandService.OnPlayerCommandReset += ResetCommands;
    }

    private void OnDisable()
    {
        PlayerInputController.SelectAbility -= SelectAbility;
        
        PlayerSelectionManager.OnSelectionChanged -= UnitSelected;
        
        GameGlobalComandService.OnPlayerCommandReset -= ResetCommands;
    }

    private void ResetCommands()
    {
        SelectAbility(0);
    }

    public void SelectAbility(int index, int targetID = 0)
    {
        UnitController unit = PlayerSelectionManager.SelectedUnit;
        if (!unit) return;
        
        BasicSkill oldAbility = unit.SkillController.CurrentSkill;

        if (!unit.SkillController.SelectSkill(index))
            return;

        GameGlobalComandService.ResetUI();

        BindAbility(oldAbility, false);
        OnTargetSwitched?.Invoke(-1);

        BasicSkill newAbility = unit.SkillController.CurrentSkill;

        bool isSame = oldAbility == newAbility;
        bool isInstant = newAbility.RequiredDataType == null;
        bool canExecute = newAbility.CanExecute();

        if (isSame && isInstant && canExecute)
        {
            ExecuteAbility();
            return;
        }

        if (!canExecute)
        {
            CameraController.SetExtraZoom(index != 0);
            FinalizeSelection(newAbility, index, canExecute);
            return;
        }

        if (newAbility.RequiredDataType == typeof(TargetData))
        {
            HandleTargetAbility(newAbility, isSame, targetID, unit);
            FinalizeSelection(newAbility, index, canExecute);
            return;
        }

        CameraController.SetExtraZoom(index != 0);
        FinalizeSelection(newAbility, index, canExecute);
    }
    
    private void HandleTargetAbility(BasicSkill ability, bool isSame, int targetID, UnitController unit)
    {
        if (isSame)
        {
            ExecuteAbility();
            return;
        }

        AbilitySwitchTarget(targetID);

        Vector3 targetPosition = (ability as ITargetSwitchable).SelectedTargetContext.Target.Target.BodyParts.Body.Transform.position;

        CameraController.AimTarget(unit.transform, targetPosition);
    }
    
    private void FinalizeSelection(BasicSkill ability, int index, bool canExecute)
    {
        BindAbility(ability, true, index != 0);
        OnAbilitySelected?.Invoke(index, canExecute);
    }

    public void AbilitySwitchToNextTarget()
    {
        if (!PlayerSelectionManager.SelectedUnit) return;
        if (PlayerSelectionManager.SelectedUnit.SkillController.CurrentSkill.RequiredDataType != typeof(TargetData)) return;
        if (PlayerSelectionManager.SelectedUnit.Combat.Targets.Count == 0) return;
        
        int step = !PlayerInputController.IsReverseModifier ? 1 : -1;
        
        ITargetSwitchable targetSwitchable = PlayerSelectionManager.SelectedUnit.SkillController.CurrentSkill as ITargetSwitchable;
        int nextIndex = (targetSwitchable.TargetIndex + targetSwitchable.TargetsCount + step) % targetSwitchable.TargetsCount;

        AbilitySwitchTarget(nextIndex);
    }
    
    public void AbilitySwitchTarget(int index)
    {
        if(!PlayerSelectionManager.SelectedUnit) return;
        if (PlayerSelectionManager.SelectedUnit.SkillController.CurrentSkill.RequiredDataType != typeof(TargetData))
        {
            SelectAbility(1, index);
        }
        
        ITargetSwitchable targetSwitchable = PlayerSelectionManager.SelectedUnit.SkillController.CurrentSkill as ITargetSwitchable;
        targetSwitchable.Switch(index);
        CameraController.AimTarget(PlayerSelectionManager.SelectedUnit.transform, targetSwitchable.SelectedTargetContext.Target.Target.BodyParts.Body.Transform.position);
        OnTargetSwitched?.Invoke(index);
    }

    private void CancelAbility()
    {
        CameraController.ChangeCameraMod(CameraMod.Tactical);
        SelectAbility(0);
    }

    private void UpdateAbilityPointData()
    {
        if (!PlayerSelectionManager.SelectedUnit || PlayerSelectionManager.SelectedUnit.State == UnitState.Engaged) return;

        PointData data = new PointData();
        data.Position = CursorController.CursorPosition;
        PlayerSelectionManager.SelectedUnit.SkillController.UpdateAbilityData(data);
    }

    public void ExecuteAbility()
    {
        if (PlayerSelectionManager.SelectedUnit.SkillController.ExecuteSkill(out bool isInstant))
        {
            GameGlobalComandService.ResetUI();
            
            OnAbilityExecuted?.Invoke();

            CameraController.SetExtraZoom(false);
            OnAbilitySelected?.Invoke(0, true);
            OnTargetSwitched?.Invoke(-1);

            if (!isInstant)
            {
                PlayerSelectionManager.SwitchToFreeUnit(true);
            }
            else
            {
                SelectAbility(0);
            }
        }
    }


    private void BindAbility(BasicSkill ability, bool bind, bool bindCancel = true)
    {
        if (!ability) return;

        Type type = ability.RequiredDataType;

        if (type == typeof(PointData))
        {
            if (bind)
            {
                CursorController.OnPositionChanged += UpdateAbilityPointData;
                PlayerInputController.PointRClick += ExecuteAbility;
                UpdateAbilityPointData();
            }
            else
            {
                CursorController.OnPositionChanged -= UpdateAbilityPointData;
                PlayerInputController.PointRClick -= ExecuteAbility;
            }
        }
        if (type == typeof(TargetData))
        {
            ITargetSwitchable targetSwitchable = ability as ITargetSwitchable;
            if (bind)
            {
                PlayerInputController.SwitchTarget += AbilitySwitchToNextTarget;
            }
            else
            {
                PlayerInputController.SwitchTarget -= AbilitySwitchToNextTarget;
            }
        }

        if (bind && bindCancel)
            PlayerInputController.Cancel += CancelAbility;
        else
            PlayerInputController.Cancel -= CancelAbility;
    }
    
    private void UnitSelected(UnitController oldUnit, UnitController unit)
    {
        if (oldUnit)
        {
            BindAbility(oldUnit.SkillController.CurrentSkill, false);
        }
        if (unit)
        {
            SelectAbility(0);
        }
    }
}