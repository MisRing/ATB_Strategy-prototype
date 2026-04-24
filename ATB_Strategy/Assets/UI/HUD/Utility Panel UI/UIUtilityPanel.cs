using UnityEngine;
using UnityEngine.UI;

public class UIUtilityPanel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _infoButton;

    [Header("Pannels")]
    [SerializeField] private GameObject _infoPannel;

    private UnitController _selectedUnit;

    private void Awake()
    {
        _infoButton.onClick.AddListener(OpenInfoPannel);
        _infoButton.gameObject.SetActive(false);
    }

    public void OpenUtilityPanel(int index)
    {
        if(index == 0 && _selectedUnit)
        {
            OpenInfoPannel();
        }
    }

    public void UnitSelected(UnitController oldUnit, UnitController unit)
    {
        _selectedUnit = unit;
        _infoButton.gameObject.SetActive(unit != null);
    }

    private void OpenInfoPannel()
    {
        _infoPannel.SetActive(!_infoPannel.activeSelf);
    }
}
