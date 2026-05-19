using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

public class TurnManager : MonoBehaviour
{
    public static float CurrentTime { get => _currentTime; }
    public static float CurrentTurnTime { get => _currentTurnTime; }
    public static int CurrentTurn { get => _currentTurn; }
    
    private static float _currentTime = 0f;
    private static float _currentTurnTime = 0f;
    private static int _currentTurn = 0;

    private static readonly float _minTimeSpeed = 0.05f;
    private static readonly float _timeStopDuration = 0.3f;


    public static readonly float TurnTime = 0.125f * 2f;
    
    private static readonly List<UnitController> _freeUnits = new List<UnitController>();
    
    private static readonly Dictionary<int, List<UnitController>> _unitsOnAction = new Dictionary<int, List<UnitController>>();
    
    public static event Action<UnitController, int> OnAbilityScheduled;
    public static event Action<int> OnAbilityResolved;
    public static event Action OnTurnEnded;


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

    public static void RemoveUnitFromBusyQ(UnitController unit)
    {
        foreach (var kvp in _unitsOnAction)
        {
            if (kvp.Value.Contains(unit))
            {
                kvp.Value.Remove(unit);
                return;
            }
        }
    }

    public static void EnterBusyQ(UnitController unit, int turnCost)
    {
        int turnKey = _currentTurn + turnCost;

        if (!_unitsOnAction.ContainsKey(turnKey))
        {
            _unitsOnAction.Add(turnKey, new List<UnitController>());
        }

        _unitsOnAction[turnKey].Add(unit);

        OnAbilityScheduled?.Invoke(unit, turnKey);
        RemoveFromWaitingQ(unit);
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
                EndTurn();
            }

            SetTimeSlowdown();

            yield return null;
        }
    }

    private void EndTurn()
    {
        OnTurnEnded?.Invoke();
        FogOfWarUtility.ResetVisibility(); //change this

        _currentTurn++;
        _lastTurnPause = false;
        if (_unitsOnAction.ContainsKey(_currentTurn))
        {
            foreach (UnitController unit in _unitsOnAction[_currentTurn])
            {
                if (unit.Owner == UnitOwner.PlayerTeam)
                {
                    _lastTurnPause = true;
                }
                EnterWaitingQ(unit);
                unit.SkillController.FinishSkill();
            }
            OnAbilityResolved?.Invoke(_currentTurn);
            _unitsOnAction.Remove(_currentTurn);
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

        if (_unitsOnAction.ContainsKey(_currentTurn + 1))
        {
            foreach (UnitController unit in _unitsOnAction[_currentTurn + 1])
            {
                if (unit.Owner == UnitOwner.PlayerTeam)
                {
                    endSlowdown = Mathf.Clamp01(remainingTime / _timeStopDuration);
                    endSlowdown = 1f - MathF.Pow(1f - endSlowdown, 3);
                    break;
                }
            }
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
