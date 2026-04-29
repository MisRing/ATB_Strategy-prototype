using System.Collections.Generic;
using UnityEngine;

public class HealthGUI : MonoBehaviour
{
    [SerializeField] private GameObject _healthPointPref;
    [SerializeField] private Vector3 _startOffset;
    [SerializeField] private Vector3 _offset;
    
    private readonly List<HealthPointGUI> _healthPoints = new List<HealthPointGUI>();
    
    public void SetHealth(UnitController unit)
    {
        for (int i = 0; i < unit.Stats.MaxHealth; i++)
        {
            GameObject point = Instantiate(_healthPointPref, transform);
            point.transform.localPosition = _startOffset + _offset * i;
            HealthPointGUI pointGUI = point.GetComponent<HealthPointGUI>();
            pointGUI.SetState(false, true);
            _healthPoints.Add(pointGUI);
        }

        int health = unit.Stats.MaxHealth;
        
        for (int i = 0; i < unit.Stats.MaxArmor; i++)
        {
            GameObject point = Instantiate(_healthPointPref, transform);
            point.transform.localPosition = _startOffset + _offset * (i + health);
            HealthPointGUI pointGUI = point.GetComponent<HealthPointGUI>();
            pointGUI.SetState(true, true);
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
            pointGUI.SetState(false, unit.Stats.Health >= i + 1);
        }

        int health = unit.Stats.MaxHealth;
        
        for (int i = 0; i < unit.Stats.MaxArmor; i++)
        {
            HealthPointGUI pointGUI = _healthPoints[i + health];
            pointGUI.SetState(true, unit.Stats.Armor >= i + 1);
        }
    }
}
