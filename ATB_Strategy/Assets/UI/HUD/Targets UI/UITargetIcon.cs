using UnityEngine;
using UnityEngine.EventSystems;

public class UITargetIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject _selectorObject;
    
    private UIUnitTargets _uiUnitTargets;
    private int _targetID;

    public void Init(UIUnitTargets uiUnitTargets, int targetID)
    {
        _uiUnitTargets = uiUnitTargets;
        _targetID = targetID;
        _selectorObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        _uiUnitTargets.SelectTarget(_targetID);
    }

    public void SetSelection(bool isSelected)
    {
        _selectorObject.SetActive(isSelected);
    }
}
