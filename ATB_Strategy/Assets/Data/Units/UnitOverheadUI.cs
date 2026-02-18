using UnityEngine;

public class UnitOverheadUI : MonoBehaviour
{
    [SerializeField] private UnitComponent _unit;
    [SerializeField] private GameObject _selectionRing;

    private void OnEnable()
    {
        _unit.OnSelectionChanged += HandleSelectionChanged;
    }

    private void OnDisable()
    {
        _unit.OnSelectionChanged -= HandleSelectionChanged;
    }

    private void HandleSelectionChanged(bool isSelected)
    {
        _selectionRing.SetActive(isSelected);
    }
}
