using System.Collections.Generic;
using UnityEngine;

public class UIAbilityController : MonoBehaviour
{
    [SerializeField] private GameObject _abilityButtonPrefab;
    private RectTransform _rectTransform;
    
    private List<UIAbilityButton> _buttons;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

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

        if (unit == null) return;

        _buttons = new List<UIAbilityButton>();
        foreach (AbilityBasic ability in unit.AbilityController.Abilities)
        {
            GameObject button = Instantiate(_abilityButtonPrefab, _rectTransform);
            UIAbilityButton uiButton = button.GetComponent<UIAbilityButton>();
            _buttons.Add(uiButton);
            uiButton.SetButton(ability);
        }
    }
}
