using UnityEngine;
using UnityEngine.UI;

public class UIAbilityPreparePanel : MonoBehaviour
{
    [SerializeField] private Button _executeButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private Text _abilityNameText;
    [SerializeField] private Text _abilityDescriptionText;
    
    private UnitAbilityController _abilityController;

    public void SetAbility(AbilityBasic ability)
    {
        _abilityController = ability.Unit.AbilityController;
        
        _executeButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();

        _executeButton.onClick.AddListener(Execute);
        _cancelButton.onClick.AddListener(Close);

        _abilityNameText.text = ability.AbilityName;
        _abilityDescriptionText.text = ability.AbilityDescription;
    }
    
    public void Execute()
    {
        _abilityController.ExecuteAbility();
    }
    
    public void Close()
    {
        _abilityController.SelectDefaultAbility();
    }
}
