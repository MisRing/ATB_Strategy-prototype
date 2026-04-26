using System.Collections.Generic;
using UnityEngine;

public class UIAbilityController : MonoBehaviour
{
    [SerializeField] private GameObject _abilityButtonPrefab;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private UIAbilityPreparePanel  _preparePanel;
    [SerializeField] private TargetAimUI _targetAimUI;
    
    private List<UIAbilityButton> _buttons;
    private UnitController _unit;
    
    private PlayerController _playerController;

    public void Init(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.OnAbilityExecuted += UpdateButtons;
    }

    private void OnDisable()
    {
        _playerController.OnAbilityExecuted -= UpdateButtons;
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
        for(int i = 1; i < unit.SkillController.SkillsCount; i++)
        {
            GameObject button = Instantiate(_abilityButtonPrefab, _rectTransform);
            UIAbilityButton uiButton = button.GetComponent<UIAbilityButton>();
            _buttons.Add(uiButton);
            uiButton.SetButton(_playerController, unit.SkillController.GetSkillByIndex(i), i);
        }
    }

    private void UpdateButtons()
    {
        if (_buttons == null || _buttons.Count == 0) return;

        foreach (UIAbilityButton button in _buttons)
        {
            button.UpdateButton();
        }
    }

    public void SetAbilityPreparePanel(int index, bool canExecute)
    {
        if(index == 0)
        {
            _preparePanel.gameObject.SetActive(false);
        }
        else
        {
            _preparePanel.gameObject.SetActive(true);
            _preparePanel.SetAbility(_unit.SkillController.GetSkillByIndex(index), _playerController, _targetAimUI, canExecute);
        }
    }
}
