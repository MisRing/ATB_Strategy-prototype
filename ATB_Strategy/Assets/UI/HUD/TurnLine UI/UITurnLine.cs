using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITurnLine : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _turnsToVisualize = 10;
    [SerializeField] private float _ySpacing = 10f;
    private float _spacing = 0;

    [Header("References")]
    [SerializeField] private RectTransform _container;
    [SerializeField] private GameObject _turnPrefab;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private Text _currentTurnText;

    private readonly Dictionary<int, List<UITurnItem>> _items = new Dictionary<int, List<UITurnItem>>();
    private readonly List<UITurnLineStand> _turnLines = new List<UITurnLineStand>();

    private int _cachedTurn;

    private void Awake()
    {
        float size = _container.rect.size.x;
        _spacing = size / _turnsToVisualize;
        for (int i = 0; i < _turnsToVisualize; i++)
        {
            GameObject obj = Instantiate(_turnPrefab, _container);
            UITurnLineStand stand = obj.GetComponent<UITurnLineStand>();
            stand.Init( (i + 1) * TurnManager.TurnTime % 1f == 0);
            stand.SetXPosition((i + 1) * _spacing);
            _turnLines.Add(stand);
        }
    }

    private void OnEnable()
    {
        TurnManager.OnAbilityScheduled += AddItem;
        TurnManager.OnAbilityResolved += RemoveItemsTurn;
    }

    private void OnDisable()
    {
        TurnManager.OnAbilityScheduled -= AddItem;
        TurnManager.OnAbilityResolved -= RemoveItemsTurn;
    }

    private void Update()
    {
        if (TimeService.TimeSpeed == 0) return;
        
        int currentTurn = TurnManager.CurrentTurn;
        float currentTurnTime = TurnManager.CurrentTurnTime / TurnManager.TurnTime;
        
        for (int i = 0; i < _turnLines.Count; i++)
        {
            float x = Mathf.Repeat(i - (currentTurn + currentTurnTime),
                _turnLines.Count - 1f) + 1f;
            float positionX  = x * _spacing;
            _turnLines[i].SetXPosition(positionX);
        }

        UpdateAllPositions();

        _currentTurnText.text = currentTurn.ToString();
    }

    private void AddItem(UnitController unit, int turn)
    {
        if (!_items.ContainsKey(turn))
        {
            _items.Add(turn, new List<UITurnItem>());
        }

        GameObject obj = Instantiate(_itemPrefab, _container);
        UITurnItem item = obj.GetComponent<UITurnItem>();

        item.Init(unit, turn);

        _items[turn].Add(item);

        float yPosition = unit.Owner == UnitOwner.PlayerTeam ? -1 : +1;

        yPosition *= _items[turn].Count * _ySpacing;

        UpdateItemPosition(item, yPosition);
    }

    private void RemoveItemsTurn(int turn)
    {
        if (!_items.ContainsKey(turn)) return;
        foreach (UITurnItem item in _items[turn])
        {
            Destroy(item.gameObject);
        }
        _items.Remove(turn);
    }

    private void UpdateAllPositions()
    {
        foreach (var kvp in _items)
        {
            foreach (var value in kvp.Value)
            {
                UpdateItemPosition(value, value.GetComponent<RectTransform>().anchoredPosition.y);
            }
        }
    }

    private void UpdateItemPosition(UITurnItem item, float yPosition)
    {
        float deltaTurn = item.Turn - TurnManager.CurrentTurn;
        float turnTime = TurnManager.CurrentTurnTime / TurnManager.TurnTime;
        float positionX = (deltaTurn - turnTime);
        positionX *= _spacing;
        
        if (deltaTurn < 0 || deltaTurn > _turnsToVisualize)
        {
            item.gameObject.SetActive(false);
            return;
        }

        item.gameObject.SetActive(true);

        RectTransform rect = item.RectTransform;
        rect.anchoredPosition = new Vector2(positionX, yPosition);
    }
}