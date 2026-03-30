using System.Collections.Generic;
using UnityEngine;

public class UIAbilityController : MonoBehaviour
{
    [SerializeField] private GameObject _abilityButtonPrefab;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private UIAbilityPreparePanel  _preparePanel;
    
    private List<UIAbilityButton> _buttons;
    private UnitController _unit;

    public void SetAbilityButtons(UnitController unit)
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
        if (_unit)
        {
            _unit.AbilityController.OnAbilitySelected -= SetAbilityPreparePanel;
        }
        _unit = unit;
        _unit.AbilityController.OnAbilitySelected += SetAbilityPreparePanel;

        _buttons = new List<UIAbilityButton>();
        for(int i = 0; i < unit.AbilityController.Abilities.Length; i++)
        {
            GameObject button = Instantiate(_abilityButtonPrefab, _rectTransform);
            UIAbilityButton uiButton = button.GetComponent<UIAbilityButton>();
            _buttons.Add(uiButton);
            uiButton.SetButton(unit.AbilityController.Abilities[i], i);
        }
    }

    private void SetAbilityPreparePanel(int index)
    {
        if(index == -1)
        {
            _preparePanel.gameObject.SetActive(false);
        }
        else
        {
            _preparePanel.gameObject.SetActive(true);
            _preparePanel.SetAbility(_unit.AbilityController.Abilities[index]);
        }
    }
}
