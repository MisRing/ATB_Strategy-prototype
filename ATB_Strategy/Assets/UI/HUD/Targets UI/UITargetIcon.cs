using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITargetIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private Image _lightningImage;
    [SerializeField] private RectTransform _selectorObject;
    [SerializeField] private RectTransform _lightning;
    [SerializeField] private Color _defaultColor = Color.red;
    [SerializeField] private Color _unprotectedColor = Color.yellow;

    private UIUnitTargets _uiUnitTargets;
    private int _targetID;

    public void Init(UIUnitTargets uiUnitTargets, int targetID, bool targetProtected)
    {
        _uiUnitTargets = uiUnitTargets;
        _targetID = targetID;

        _icon.color = targetProtected ? _defaultColor : _unprotectedColor;
        //_lightningImage.color = targetProtected ? _defaultColor : _unprotectedColor;

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
