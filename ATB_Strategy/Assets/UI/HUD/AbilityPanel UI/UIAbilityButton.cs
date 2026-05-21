using UnityEngine;
using UnityEngine.UI;

public class UIAbilityButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Text _idText;
    [SerializeField] private Image _cooldownImage;
    [SerializeField] private Text _cooldownText;

    private BasicSkill _ability;
    private int _abilityIndex = -1;
    private PlayerController _playerController;

    public void SetButton(PlayerController playerController, BasicSkill ability, int index)
    {
        _abilityIndex = index;
        _ability = ability;

        _playerController = playerController;
        _button.onClick.AddListener(SelectThisAbility);

        UpdateButton();
    }

    public void UpdateButton()
    {
        _idText.text = _abilityIndex.ToString();

        _cooldownImage.fillAmount = (float)_ability.CurrentCooldown / (float)_ability.MaxCooldown;
        _cooldownText.text = _ability.CurrentCooldown.ToString();
        _cooldownImage.gameObject.SetActive(_ability.CurrentCooldown > 0);

        _buttonImage.sprite = _ability.SkillIcon;
    }

    private void SelectThisAbility()
    {
        _playerController.SelectAbility(_abilityIndex);
    }
}
