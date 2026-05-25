using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthGUI : MonoBehaviour
{
    [SerializeField] private GameObject _healthPointPref;
    [SerializeField] private TextMeshPro _header;
    [SerializeField] private Color _playerColor = Color.cyan;
    [SerializeField] private Color _enemyColor = Color.darkRed;
    [SerializeField] private Vector3 _startOffset;
    [SerializeField] private Vector3 _offset;
    
    private readonly List<HealthPointGUI> _healthPoints = new List<HealthPointGUI>();
    
    public void SetHealth(UnitController unit)
    {
        _header.text = unit.Stats.ID.ToString() + " " + unit.Stats.Name;
        _header.color = unit.Owner == UnitOwner.PlayerTeam ? _playerColor : _enemyColor;
        
        for (int i = 0; i < unit.Stats.MaxHealth; i++)
        {
            GameObject point = Instantiate(_healthPointPref, transform);
            point.transform.localPosition = _startOffset + _offset * i;
            HealthPointGUI pointGUI = point.GetComponent<HealthPointGUI>();
            pointGUI.SetState(false, true, unit.Owner != UnitOwner.PlayerTeam);
            _healthPoints.Add(pointGUI);
        }

        int health = unit.Stats.MaxHealth;
        
        for (int i = 0; i < unit.Stats.MaxArmor; i++)
        {
            GameObject point = Instantiate(_healthPointPref, transform);
            point.transform.localPosition = _startOffset + _offset * (i + health);
            HealthPointGUI pointGUI = point.GetComponent<HealthPointGUI>();
            pointGUI.SetState(true, true, unit.Owner != UnitOwner.PlayerTeam);
            _healthPoints.Add(pointGUI);
        }
    }

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void ChangeHealth(UnitController unit)
    {
        for (int i = 0; i < unit.Stats.MaxHealth; i++)
        {
            HealthPointGUI pointGUI = _healthPoints[i];
            pointGUI.SetState(false, unit.Stats.Health >= i + 1, unit.Owner != UnitOwner.PlayerTeam);
        }

        int health = unit.Stats.MaxHealth;
        
        for (int i = 0; i < unit.Stats.MaxArmor; i++)
        {
            HealthPointGUI pointGUI = _healthPoints[i + health];
            pointGUI.SetState(true, unit.Stats.Armor >= i + 1, unit.Owner != UnitOwner.PlayerTeam);
        }
    }
}
