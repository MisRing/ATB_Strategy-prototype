using UnityEngine;
using UnityEngine.UI;

public class UIAbilityButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _buttonImage;

    private int _abilityIndex = -1;
    private UnitAbilityController _abilityController;

    public void SetButton(AbilityBasic ability, int index)
    {
        _abilityIndex = index;
        _abilityController = ability.Unit.AbilityController;
        _buttonImage.sprite = ability.AbilityIcon;
        _button.onClick.AddListener(SetAbility);
    }

    public void SetAbility()
    {
        _abilityController.SelectAbility(_abilityIndex);
    }
}
