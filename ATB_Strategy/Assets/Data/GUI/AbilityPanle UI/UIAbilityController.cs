using System.Collections.Generic;
using UnityEngine;

public class UIAbilityController : MonoBehaviour
{
    [SerializeField] private GameObject _abilityButtonPrefab;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private UIAbilityPreparePanel  _preparePanel;
    
    private List<UIAbilityButton> _buttons;
    private UnitController _unit;
    
    private PlayerController _playerController;

    public void Init(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public void SetAbilityButtons(UnitController oldUnit, UnitController unit)
    {
        if (_buttons != null)
        {
            foreach (UIAbilityButton button in _buttons)
            {
                Destroy(button.gameObject);
            }
            _buttons.Clear();
        }

        if (!unit) return;
        _unit = unit;

        _buttons = new List<UIAbilityButton>();
        for(int i = 0; i < unit.SkillController.Skills.Length; i++)
        {
            GameObject button = Instantiate(_abilityButtonPrefab, _rectTransform);
            UIAbilityButton uiButton = button.GetComponent<UIAbilityButton>();
            _buttons.Add(uiButton);
            uiButton.SetButton(_playerController, unit.SkillController.Skills[i], i);
        }
    }

    public void SetAbilityPreparePanel(int index)
    {
        if(index == -1)
        {
            _preparePanel.gameObject.SetActive(false);
        }
        else
        {
            _preparePanel.gameObject.SetActive(true);
            _preparePanel.SetAbility(_unit.SkillController.Skills[index], _playerController);
        }
    }
}
