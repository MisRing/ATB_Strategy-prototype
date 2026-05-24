using UnityEngine;
using UnityEngine.UI;

public class UITurnLineStand : MonoBehaviour
{
    [SerializeField] private GameObject _normalLine;
    [SerializeField] private GameObject _numericLine;
    [SerializeField] private RectTransform _rect;

    private bool _numeric;

    public void Init(bool numeric)
    {
        _numeric = numeric;
        _normalLine.SetActive(!_numeric);
        _numericLine.SetActive(_numeric);
    }

    public void SetXPosition(float x)
    {
        _rect.anchoredPosition = new Vector2(x, 0);
    }
}
