using UnityEngine;
using UnityEngine.UI;

public class UITurnItem : MonoBehaviour
{
    public int Turn { get; private set; }
    public BasicSkill Ability { get; private set; }

    public RectTransform RectTransform { get; private set; }

    [SerializeField] private Image _icon;
    [SerializeField] private Image _ownerColor;

    public void Init(UnitController unit, int turn)
    {
        Turn = turn;

        RectTransform = GetComponent<RectTransform>();

        _ownerColor.color = unit.Owner == UnitOwner.PlayerTeam
            ? Color.green
            : Color.red;
    }
}
