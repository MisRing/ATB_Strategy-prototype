using System;
using UnityEngine;
using UnityEngine.UI;

public class UIAbilityPreparePanel : MonoBehaviour
{
    [SerializeField] private GameObject _cantExecutePanel;
    [SerializeField] private Button _executeButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Text _abilityNameText;
    [SerializeField] private Text _abilityDescriptionText;
    
    private PlayerController _playerController;
    private TargetAimUI _targetAimUI;
    private ITargetSwitchable _targetSwitchable;

    public void SetAbility(BasicSkill ability, PlayerController playerController, TargetAimUI targetAimUI, bool canExecute)
    {
        _cantExecutePanel.SetActive(!canExecute);
        
        _targetAimUI = targetAimUI;
        _playerController = playerController;
        
        _executeButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        _executeButton.onClick.AddListener(Execute);
        _cancelButton.onClick.AddListener(Close);

        _abilityNameText.text = ability.SkillName;
        _abilityDescriptionText.text = ability.SkillDescription;

        if (ability.RequiredDataType == typeof(TargetData))
        {
            _targetSwitchable = ability as ITargetSwitchable;
            
            //_targetSwitchable.OnTargetSwitched += SetTarget;
            _playerController.OnTargetSwitched += SetTarget;
            
            if(_targetSwitchable.TargetsCount >= 1)
            {
                SetTarget(0);
            }
        }
    }



    private void OnDisable()
    {
        _targetAimUI.gameObject.SetActive(false);

        _playerController.OnTargetSwitched -= SetTarget;
    }

    private void SetTarget(int index)
    {
        if (index == -1)
        {
            _targetAimUI.gameObject.SetActive(false);
            return;
        }
        CombatContext combatContext = _targetSwitchable.CurrentTarget;
        _targetAimUI.gameObject.SetActive(true);
        _targetAimUI.SetTarget(combatContext.Target.Target.BodyParts.Body.Transform, combatContext.HitChance, combatContext.CritChance);
    }

    private void Execute()
    {
        _playerController.ExecuteAbility();
    }

    private void Close()
    {
        _playerController.SelectAbility(0);
    }
}
