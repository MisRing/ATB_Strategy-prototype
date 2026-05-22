using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIUnitPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _numberText;
    [SerializeField] private Image _icon;
    [SerializeField] private UIBar _healthBar;
    [SerializeField] private UIBar _armorBar;
    [SerializeField] private float _selectedScale = 1.4f;
    [SerializeField] private float _scaleSpeed = 10f;

    [HideInInspector] public UnitController Unit;
    private UISquadPanel _squadPanel;
    private RectTransform _rectTransform;
    private float _targetScale;

    public void Init(UnitController unit, int number, UISquadPanel squadPanel)
    {
        Unit = unit;
        _squadPanel = squadPanel;
        _rectTransform = GetComponent<RectTransform>();

        _nameText.text = Unit.Stats.Name;
        _numberText.text = number.ToString();
        _icon.sprite = Unit.Stats.Icon;

        Unit.Stats.Health.OnValueChanged += DrawHealth;
        Unit.Stats.MaxHealth.OnValueChanged += DrawHealth;

        Unit.Stats.Armor.OnValueChanged += DrawArmor;
        Unit.Stats.MaxArmor.OnValueChanged += DrawArmor;
        
        _targetScale = 1f;
        _rectTransform.localScale = _targetScale * Vector3.one;

        DrawHealth(0);
        DrawArmor(0);
    }

    private void OnDisable()
    {
        if (!Unit) return;

        Unit.Stats.Health.OnValueChanged -= DrawHealth;
        Unit.Stats.MaxHealth.OnValueChanged -= DrawHealth;

        Unit.Stats.Armor.OnValueChanged -= DrawArmor;
        Unit.Stats.MaxArmor.OnValueChanged -= DrawArmor;
    }

    private void DrawHealth(int value)
    {
        _healthBar.SetValue(Unit.Stats.Health, Unit.Stats.MaxHealth);
    }

    private void DrawArmor(int value)
    {
        _armorBar.SetValue(Unit.Stats.Armor, Unit.Stats.MaxArmor);
    }

    public void SetSelection(bool selection)
    {
        _healthBar.SetActiveText(selection);
        _armorBar.SetActiveText(selection);

        _targetScale = selection ? _selectedScale : 1f;
    }

    private void Update()
    {
        Vector3 scale = _rectTransform.localScale + (Vector3.one * _targetScale - _rectTransform.localScale).normalized * (Time.deltaTime * _scaleSpeed);
        scale = new Vector3(Mathf.Clamp(scale.x, 1f, _selectedScale),  Mathf.Clamp(scale.y, 1f, _selectedScale), Mathf.Clamp(scale.z, 1f, _selectedScale));
        _rectTransform.localScale = scale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _squadPanel.SelectUnit(Unit);
    }
}
