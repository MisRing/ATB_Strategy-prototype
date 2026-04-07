using UnityEngine;

public class UnitOverheadUI : MonoBehaviour
{
    [SerializeField] private UnitController _unit;
    [SerializeField] private GameObject _selectionRing;
    [SerializeField] private HealthGUI _healthGUI;

    private void Start()
    {
        _healthGUI.SetHealth(_unit);
        _unit.UnitStats.Health.OnValueChanged += HealthChanged; //!
        _unit.UnitStats.Armor.OnValueChanged += HealthChanged;  //!
    }
    private void OnEnable()
    {
        _unit.OnSelectionChanged += HandleSelectionChanged;
        _healthGUI.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _unit.OnSelectionChanged -= HandleSelectionChanged;
        _healthGUI.gameObject.SetActive(false);
        _unit.UnitStats.Health.OnValueChanged -= HealthChanged;
        _unit.UnitStats.Armor.OnValueChanged -= HealthChanged;
    }

    private void HandleSelectionChanged(bool isSelected)
    {
        _selectionRing.SetActive(isSelected);
    }

    private void HealthChanged(int value)
    {
        _healthGUI.ChangeHealth(_unit);
    }
}
