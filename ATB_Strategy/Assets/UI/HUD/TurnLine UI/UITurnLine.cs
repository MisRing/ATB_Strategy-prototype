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
    private readonly List<RectTransform> _turnLines = new List<RectTransform>();

    private int _cachedTurn;

    private void Awake()
    {
        float size = _container.sizeDelta.x;
        _spacing = size / _turnsToVisualize;
        for (int i = 0; i < _turnsToVisualize; i++)
        {
            GameObject obj = Instantiate(_turnPrefab, _container);
            RectTransform  rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(i * _spacing, 0);
            _turnLines.Add(rect);
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
        float turnTime = TurnManager.CurrentTurnTime / TurnManager.TurnTime;
        
        for (int i = 0; i < _turnLines.Count; i++)
        {
            float positionX = (i + 1 - turnTime);
            positionX *= _spacing;
            _turnLines[i].anchoredPosition = new Vector2(positionX, 0);
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