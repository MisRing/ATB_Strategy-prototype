using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITurnItem : MonoBehaviour
{
    public int Turn { get; private set; }
    public BasicSkill Ability { get; private set; }

    public RectTransform RectTransform { get; private set; }

    [SerializeField] private Image _icon;
    [SerializeField] private Image _background;
    [SerializeField] private Image _turn;
    [SerializeField] private TextMeshProUGUI _id;
    
    [SerializeField] private Color _playerColor = Color.aliceBlue;
    [SerializeField] private Color _enemyColor = Color.red;
    [SerializeField] private Color _backgroundColor = Color.red;
    
    [SerializeField] private float _ySpacing = 50f;
    [SerializeField] private float _turnYSpacing = 12f;

    private RectTransform _backgroundRect;
    private RectTransform _turnRect;
    //[SerializeField] private Image _ownerColor;

    public void Init(UnitController unit, int turn, int inRow)
    {
        Turn = turn;
        _backgroundRect = _background.GetComponent<RectTransform>();
        _turnRect = _turn.GetComponent<RectTransform>();
        _turn.color = unit.Owner == UnitOwner.PlayerTeam ? _playerColor : _enemyColor;

        RectTransform = GetComponent<RectTransform>();

        _background.color = unit.Owner == UnitOwner.PlayerTeam ? Color.white : _backgroundColor;
        _icon.sprite = unit.Stats.Icon;
        _id.text = unit.Stats.ID.ToString();
    }

    public void UpdatePosition(float xPosition, int inRow)
    {
        RectTransform.anchoredPosition = new Vector2(xPosition, 0);
        _backgroundRect.anchoredPosition = new Vector2(0, inRow * _ySpacing * -1 - 14);
        _turnRect.anchoredPosition = new Vector2(0, inRow * _turnYSpacing);
    }
}
