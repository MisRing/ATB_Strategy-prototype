using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITurnLineStand : MonoBehaviour
{
    [SerializeField] private GameObject _normalLine;
    [SerializeField] private GameObject _numericLine;
    [SerializeField] private TextMeshProUGUI _turnNumber;
    [SerializeField] private RectTransform _rect;

    private bool _numeric;

    public void Init(bool numeric, int turn)
    {
        _numeric = numeric;
        _turnNumber.text = turn.ToString();
        _normalLine.SetActive(!_numeric);
        _numericLine.SetActive(_numeric);
        _turnNumber.gameObject.SetActive(_numeric);
    }

    public void SetXPosition(float x, int turn)
    {
        _rect.anchoredPosition = new Vector2(x, 0);
        
        if (!_numeric) return;
        
        _turnNumber.text = turn.ToString();
    }
}
