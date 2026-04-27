using System;
using UnityEngine;
using UnityEngine.UI;

public class UIUtilityPanel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _infoButton;

    [Header("Panels")]
    [SerializeField] private UIStatPanel _infoPanel;

    private UnitController _selectedUnit;

    private void Awake()
    {
        _infoButton.onClick.AddListener(OpenInfoPanel);
        _infoButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameGlobalComandService.OnUICommandReset += ClosePanels;
    }

    private void OnDisable()
    {
        GameGlobalComandService.OnUICommandReset -= ClosePanels;
    }

    private void ClosePanels()
    {
        _infoPanel.gameObject.SetActive(false);
    }

    public void OpenUtilityPanel(int index)
    {
        if(index == 0 && _selectedUnit)
        {
            OpenInfoPanel();
        }
    }

    public void UnitSelected(UnitController oldUnit, UnitController unit)
    {
        _selectedUnit = unit;
        _infoButton.gameObject.SetActive(unit != null);
    }

    private void OpenInfoPanel()
    {
        GameGlobalComandService.ResetPlayerCommands();
        
        _infoPanel.gameObject.SetActive(true);
        _infoPanel.SetStatPanel(_selectedUnit);
    }
}
