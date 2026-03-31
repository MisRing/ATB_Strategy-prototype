using UnityEngine;
using UnityEngine.UI;

public class UIAbilityButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _buttonImage;

    private int _abilityIndex = -1;
    private PlayerController _playerController;

    public void SetButton(PlayerController playerController, AbilityBasic ability, int index)
    {
        _abilityIndex = index;
        _playerController = playerController;
        _buttonImage.sprite = ability.AbilityIcon;
        _button.onClick.AddListener(SetAbility);
    }

    public void SetAbility()
    {
        _playerController.SelectAbility(_abilityIndex);
    }
}
