using UnityEngine;
using UnityEngine.UI;

public class UIAbilityButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _buttonImage;

    private AbilityBasic _ability;

    public void SetButton(AbilityBasic ability)
    {
        _ability = ability;

        _buttonImage.sprite = _ability.AbilityIcon;
    }
}
