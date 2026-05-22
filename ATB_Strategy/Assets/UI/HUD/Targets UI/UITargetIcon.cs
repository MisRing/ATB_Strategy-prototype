using UnityEngine;
using UnityEngine.EventSystems;

public class UITargetIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RectTransform _selectorObject;
    [SerializeField] private RectTransform _lightning;
    
    private UIUnitTargets _uiUnitTargets;
    private int _targetID;

    public void Init(UIUnitTargets uiUnitTargets, int targetID)
    {
        _uiUnitTargets = uiUnitTargets;
        _targetID = targetID;
        _selectorObject.gameObject.SetActive(false);
        _lightning.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        _uiUnitTargets.SelectTarget(_targetID);
    }

    public void SetSelection(bool isSelected)
    {
        _selectorObject.gameObject.SetActive(isSelected);
        _lightning.gameObject.SetActive(isSelected);
    }

    private void Update()
    {
        if (!_lightning.gameObject.activeSelf) return;
        float pulse = (1f + Mathf.Sin(Time.time * 2f) * 0.05f);
        _lightning.localScale = Vector3.one *  pulse;
        _selectorObject.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30f * pulse);
        _selectorObject.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30f * pulse);
    }
}
