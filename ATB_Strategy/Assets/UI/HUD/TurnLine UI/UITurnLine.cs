using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITurnLine : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _turnsToVisualize = 10;
    [SerializeField] private float _ySpacing = 50f;
    private float _spacing = 0;

    [Header("References")]
    [SerializeField] private RectTransform _container;
    [SerializeField] private GameObject _turnPrefab;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private TextMeshProUGUI _currentTurnText;

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
            stand.Init( (i + 1) * TurnManager.TurnTime % 1f == 0, i + 1);
            stand.SetXPosition((i + 1) * _spacing, i + 1);
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
        _currentTurnText.text = TurnManager.CurrentTurn.ToString();
        if (TimeService.TimeSpeed == 0) return;
        
        int currentTurn = TurnManager.CurrentTurn;
        float currentTurnTime = Mathf.Round((TurnManager.CurrentTurnTime / TurnManager.TurnTime) * 100f) / 100f;
        
        for (int i = 0; i < _turnLines.Count; i++)
        {
            float x = Mathf.Repeat(i + 1 - (currentTurn + currentTurnTime),
                _turnLines.Count);
            x = x == 0 ? _turnLines.Count : x;

            float positionX = x * _spacing;
            _turnLines[i].SetXPosition(positionX, currentTurn + Mathf.FloorToInt(x) + Mathf.CeilToInt(currentTurnTime));
        }

        UpdateAllPositions();
    }

    private void AddItem(UnitController unit, int turn)
    {
        if(unit.State == UnitState.Waiting) return;
        if (!_items.ContainsKey(turn))
        {
            _items.Add(turn, new List<UITurnItem>());
        }

        GameObject obj = Instantiate(_itemPrefab, _container);
        UITurnItem item = obj.GetComponent<UITurnItem>();

        item.Init(unit, turn, _items[turn].IndexOf(item));

        _items[turn].Add(item);
        
        UpdateItemPosition(item, turn);
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
                UpdateItemPosition(value, kvp.Key);
            }
        }
    }

    private void UpdateItemPosition(UITurnItem item, int turn)
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

        item.UpdatePosition(positionX, _items[turn].IndexOf(item));
    }
}