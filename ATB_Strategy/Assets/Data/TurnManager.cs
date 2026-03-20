using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    private static float _currentTime = 0f;
    private static float _currentTurnTime = 0f;
    private static int _currentTurn = 0;

    private static float _minTimeSpeed = 0.1f;

    public static readonly float TurnTime = 0.15f;

    public static event Action<UnitController> OnUnitEnterExitQ;
    
    private static List<UnitController> _freeUnits = new List<UnitController>();
    
    private static Dictionary<int, List<AbilityBasic>> _abilitiesOnAction = new Dictionary<int, List<AbilityBasic>>();

    private void Start()
    {
        StartCoroutine(TurnTick());
    }
    
    public static void EnterWaitingQ(UnitController unit)
    {
        if(_freeUnits.Contains(unit)) return;

        _freeUnits.Add(unit);
        TimeService.SetTimeSpeed(0);
    }
    
    private static void RemoveFromWaitingQ(UnitController unit)
    {
        if(!_freeUnits.Contains(unit)) return;

        _freeUnits.Remove(unit);
        if (_freeUnits.Count == 0)
        {
            TimeService.SetTimeSpeed(_minTimeSpeed);
        }
    }
    
    public static void EnterBusyQ(AbilityBasic abilityBasic, int turnCost)
    {
        int turnKey = _currentTurn + turnCost;

        if (!_abilitiesOnAction.ContainsKey(turnKey))
        {
            _abilitiesOnAction.Add(turnKey, new List<AbilityBasic>());
        }
        
        _abilitiesOnAction[turnKey].Add(abilityBasic);
        
        RemoveFromWaitingQ(abilityBasic.Unit);
    }

    private IEnumerator TurnTick()
    {
        while (true)
        {
            float deltaTime = TimeService.TimeSpeed * Time.deltaTime;
            _currentTime += deltaTime;
            _currentTurnTime += deltaTime;

            if (TimeService.TimeSpeed != 0f)
            {
                if (_currentTurnTime < TurnTime / 2f)
                {
                    TimeService.SetTimeSpeed(Mathf.Lerp(TimeService.TimeSpeed, 1f, _currentTurnTime / (TurnTime / 2f)));
                }
                else if (_currentTurnTime > TurnTime / 2f && _abilitiesOnAction.ContainsKey(_currentTurn + 1))
                {
                    float t = (_currentTurnTime - (TurnTime / 2f)) / (TurnTime / 2f);
                    TimeService.SetTimeSpeed(Mathf.Lerp(TimeService.TimeSpeed, _minTimeSpeed, t));
                }
            }

            if (_currentTurnTime >= TurnTime)
            {
                _currentTurnTime -= TurnTime;
                _currentTurn++;

                if (_abilitiesOnAction.ContainsKey(_currentTurn))
                {
                    OnUnitEnterExitQ?.Invoke(_abilitiesOnAction[_currentTurn][0].Unit);
                    foreach (AbilityBasic ability in _abilitiesOnAction[_currentTurn])
                    {
                        ability.FinishExecute();
                        EnterWaitingQ(ability.Unit);
                    }
                    _abilitiesOnAction.Remove(_currentTurn);
                }
            }
            
            yield return null;
        }
    }
    
    void OnGUI() {
        GUILayout.BeginArea(new Rect(Screen.width - 400, 0, 400, Screen.height));
        GUILayout.Label("Current Time: " + _currentTime + "\n Current Turn: " + _currentTurn + "\n Current Time Speed: " + TimeService.TimeSpeed);
        GUILayout.EndArea();
    }
}
