using UnityEngine;
using UnityEngine.UI;

public class UIAbilityPreparePanel : MonoBehaviour
{
    [SerializeField] private Button _executeButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Text _abilityNameText;
    [SerializeField] private Text _abilityDescriptionText;
    
    private PlayerController _playerController;

    public void SetAbility(BasicSkill ability, PlayerController playerController)
    {
        _playerController = playerController;
        
        _executeButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        _executeButton.onClick.AddListener(Execute);
        _cancelButton.onClick.AddListener(Close);

        _abilityNameText.text = ability.SkillName;
        _abilityDescriptionText.text = ability.SkillDescription;
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
