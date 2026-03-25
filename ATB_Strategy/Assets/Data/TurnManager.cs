using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    private static float _currentTime = 0f;
    private static float _currentTurnTime = 0f;
    private static int _currentTurn = 0;

    private static float _minTimeSpeed = 0.05f;
    private static float _timeStopDuration = 0.3f;


    public static readonly float TurnTime = 0.125f;

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
    private bool _lastTurnPause = true;

    private IEnumerator TurnTick()
    {
        while (true)
        {
            _currentTime += TimeService.TimeSpeedDelta;
            _currentTurnTime += TimeService.TimeSpeedDelta;

            if (_currentTurnTime >= TurnTime)
            {
                _currentTurnTime -= TurnTime;
                _currentTurn++;

                if (_abilitiesOnAction.ContainsKey(_currentTurn))
                {
                    _lastTurnPause = true;

                    OnUnitEnterExitQ?.Invoke(_abilitiesOnAction[_currentTurn][0].Unit);
                    foreach (AbilityBasic ability in _abilitiesOnAction[_currentTurn])
                    {
                        ability.FinishExecute();
                        EnterWaitingQ(ability.Unit);
                    }
                    _abilitiesOnAction.Remove(_currentTurn);
                }
                else
                {
                    _lastTurnPause = false;
                }
            }

            SetTimeSlowdown();

            yield return null;
        }
    }


    private void SetTimeSlowdown()
    {
        if (TimeService.TimeSpeed == 0f) return;

        float passedTime = _currentTurnTime;
        float remainingTime = TurnTime - _currentTurnTime;

        float startSlowdown = 1f;
        float endSlowdown = 1f;

        if (_lastTurnPause)
        {
            startSlowdown = Mathf.Clamp01(passedTime / _timeStopDuration);
            startSlowdown = 1f - MathF.Pow(1f - startSlowdown, 3);
        }

        if (_abilitiesOnAction.ContainsKey(_currentTurn + 1))
        {
            endSlowdown = Mathf.Clamp01(remainingTime / _timeStopDuration);
            endSlowdown = 1f - MathF.Pow(1f - endSlowdown, 3);
        }

        float slowdown = Mathf.Min(startSlowdown, endSlowdown);
        slowdown *= TimeService.DefaultTimeSpeed;

        TimeService.SetTimeSpeed(Mathf.Max(slowdown, _minTimeSpeed));
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 400, 0, 400, Screen.height));
        GUILayout.Label("Current Time: " + _currentTime + "\n Current Turn: " + _currentTurn + "\n Current Time Speed: " + TimeService.TimeSpeed);
        GUILayout.EndArea();
    }
}
