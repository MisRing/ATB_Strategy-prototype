using UnityEngine;

public class UnitOverheadUI : MonoBehaviour
{
    [SerializeField] private UnitController _unit;
    [SerializeField] private GameObject _selectionRing;
    [SerializeField] private HealthGUI _healthGUI;

    private void Awake()
    {
        _healthGUI.SetHealth(_unit);
    }
    
    private void OnEnable()
    {
        _unit.OnSelectionChanged += HandleSelectionChanged;
        _healthGUI.gameObject.SetActive(true);
        _unit.Stats.Health.OnValueChanged += HealthChanged;
        _unit.Stats.Armor.OnValueChanged += HealthChanged;
        
        _unit.Combat.OnUnitDie += DisableHealthGUI;
    }

    private void OnDisable()
    {
        _unit.OnSelectionChanged -= HandleSelectionChanged;
        _healthGUI.gameObject.SetActive(false);
        _unit.Stats.Health.OnValueChanged -= HealthChanged;
        _unit.Stats.Armor.OnValueChanged -= HealthChanged;
        
        _unit.Combat.OnUnitDie -= DisableHealthGUI;
    }

    private void HandleSelectionChanged(bool isSelected)
    {
        _selectionRing.SetActive(isSelected);
    }

    private void DisableHealthGUI()
    {
        _healthGUI.gameObject.SetActive(false);
    }

    private void HealthChanged(int value)
    {
        _healthGUI.ChangeHealth(_unit);
    }
}
