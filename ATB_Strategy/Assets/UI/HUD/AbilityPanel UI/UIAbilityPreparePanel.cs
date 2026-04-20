using System;
using UnityEngine;
using UnityEngine.UI;

public class UIAbilityPreparePanel : MonoBehaviour
{
    [SerializeField] private Button _executeButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Text _abilityNameText;
    [SerializeField] private Text _abilityDescriptionText;
    
    private PlayerController _playerController;
    private TargetAimUI _targetAimUI;
    private ITargetSwitchable _targetSwitchable;

    public void SetAbility(BasicSkill ability, PlayerController playerController, TargetAimUI targetAimUI)
    {
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
            
            _targetSwitchable.OnTargetSwitched += SetTarget;
            
            if(_targetSwitchable.TargetsCount > 1)
            {
                SetTarget(_targetSwitchable.CurrentTarget);
            }
        }
    }

    

    private void OnDisable()
    {
        _targetAimUI.gameObject.SetActive(false);
        if (_targetSwitchable != null)
        {
            _targetSwitchable.OnTargetSwitched -= SetTarget;
        }
    }

    private void SetTarget(HitInfo hitInfo)
    {
        _targetAimUI.gameObject.SetActive(true);
        _targetAimUI.SetTarget(hitInfo.Target.BodyParts.Body.Transform, hitInfo.HitChance);
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
