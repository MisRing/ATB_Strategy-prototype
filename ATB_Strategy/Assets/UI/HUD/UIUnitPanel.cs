using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIUnitPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Image _icon;
    [SerializeField] private UIBar _healthBar;
    [SerializeField] private UIBar _armorBar;
    [SerializeField] private Vector2 _normalSize;
    [SerializeField] private Vector2 _selectedSize;

    [HideInInspector] public UnitController Unit;
    private UISquadPanel _squadPanel;
    private RectTransform _rectTransform;

    public void Init(UnitController unit, UISquadPanel squadPanel)
    {
        Unit = unit;
        _squadPanel = squadPanel;
        _rectTransform = GetComponent<RectTransform>();

        _nameText.text = Unit.UnitStats.Name;
        _icon.sprite = Unit.UnitStats.Icon;

        Unit.UnitStats.Health.OnValueChanged += DrawHealth;
        Unit.UnitStats.MaxHealth.OnValueChanged += DrawHealth;

        Unit.UnitStats.Armor.OnValueChanged += DrawArmor;
        Unit.UnitStats.MaxArmor.OnValueChanged += DrawArmor;

        DrawHealth(0);
        DrawArmor(0);
    }

    private void OnDisable()
    {
        if (!Unit) return;

        Unit.UnitStats.Health.OnValueChanged -= DrawHealth;
        Unit.UnitStats.MaxHealth.OnValueChanged -= DrawHealth;

        Unit.UnitStats.Armor.OnValueChanged -= DrawArmor;
        Unit.UnitStats.MaxArmor.OnValueChanged -= DrawArmor;
    }

    private void DrawHealth(int value)
    {
        _healthBar.SetValue(Unit.UnitStats.Health, Unit.UnitStats.MaxHealth);
    }

    private void DrawArmor(int value)
    {
        _armorBar.SetValue(Unit.UnitStats.Armor, Unit.UnitStats.MaxArmor);
    }

    public void SetSelection(bool selection)
    {
        _healthBar.SetActiveText(selection);
        _armorBar.SetActiveText(selection);

        if (selection)
        {
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _selectedSize.x);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _selectedSize.y);
        }
        else
        {
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _normalSize.x);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _normalSize.y);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _squadPanel.SelectUnit(Unit);
    }
}
