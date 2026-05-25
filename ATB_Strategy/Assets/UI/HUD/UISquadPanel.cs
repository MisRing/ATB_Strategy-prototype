using System.Collections.Generic;
using UnityEngine;

public class UISquadPanel : MonoBehaviour
{
    [SerializeField] private GameObject _unitPanelPrefab;

    private List<UIUnitPanel> _unitPanels;
    private RectTransform _rectTransform;
    private PlayerSelectionManager _selectionManager;

    public void Init(PlayerSelectionManager manager)
    {
        _selectionManager = manager;
        _rectTransform = GetComponent<RectTransform>();

        _unitPanels = new List<UIUnitPanel>();
        for(int i = 0; i < _selectionManager.Units.Count; i++)
        {
            UIUnitPanel panel = Instantiate(_unitPanelPrefab, _rectTransform).GetComponent<UIUnitPanel>();
            panel.Init(_selectionManager.Units[i], _selectionManager.Units[i].Stats.ID, this);
            _unitPanels.Add(panel);
        }

        _selectionManager.OnSelectionChanged += SelectUnitPanel;
    }

    private void OnDisable()
    {
        if (!_selectionManager) return;

        _selectionManager.OnSelectionChanged -= SelectUnitPanel;
    }

    public void SelectUnit(UnitController unit)
    {
        _selectionManager.SelectUnit(unit);
    }

    private void SelectUnitPanel(UnitController oldUnit, UnitController unit)
    {
        foreach(UIUnitPanel panel in _unitPanels)
        {
            panel.SetSelection(false);
        }

        UIUnitPanel selectedPanel = _unitPanels.Find(x => x.Unit == unit);
        selectedPanel?.SetSelection(true);
    }
}
