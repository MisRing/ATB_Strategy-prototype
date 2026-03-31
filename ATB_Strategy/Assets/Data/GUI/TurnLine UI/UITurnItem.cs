using UnityEngine;
using UnityEngine.UI;

public class UITurnItem : MonoBehaviour
{
    public int Turn { get; private set; }
    public AbilityBasic Ability { get; private set; }

    public RectTransform RectTransform { get; private set; }

    [SerializeField] private Image _icon;
    [SerializeField] private Image _ownerColor;

    public void Init(AbilityBasic ability, int turn)
    {
        Ability = ability;
        Turn = turn;

        RectTransform = GetComponent<RectTransform>();

        if (_icon)
            _icon.sprite = ability.AbilityIcon;

        if (_ownerColor)
        {
            _ownerColor.color = ability.Unit.Owner == UnitOwner.Player
                ? Color.green
                : Color.red;
        }
    }
}
