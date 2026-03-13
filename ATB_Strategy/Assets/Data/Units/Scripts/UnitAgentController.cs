using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitAgentController : MonoBehaviour
{
    [SerializeField] private float _pathEndThreshold = 0.05f;
    [SerializeField] private float _aceleration = 1f;
    public Vector3 Velocity { get { return _agent.velocity / _agent.speed; } }

    private NavMeshAgent _agent;
    private UnitController _unit;

    public event Action OnMoveComplete;

    public void Init(UnitController unit, GridTile startTile)
    {
        _agent = GetComponent<NavMeshAgent>();
        //_agent.autoRepath = false;
        //_agent.updatePosition = false;
        //_agent.updateRotation = false;
        _agent.acceleration = 100000f;

        _unit = unit;
        _agent.Warp(GridParameters.LevelGrid.GetTileWorldPos(startTile));

        _agent.speed = _unit.UnitStats.Speed * TimeService.TimeSpeed;

        TimeService.OnTimeSpeedChanged += SetAgentSpeed;//
    }

    //private void OnEnable()
    //{
    //    TimeService.OnTimeSpeedChanged += SetAgentSpeed;
    //}

    private void OnDisable()
    {
        TimeService.OnTimeSpeedChanged -= SetAgentSpeed;
    }

    private void SetAgentSpeed(float timeSpeed)
    {
        _agent.speed = _unit.UnitStats.Speed * timeSpeed;
    }

    public bool CalculatePath(ref PathData pathData, Vector3 targetPoint)
    {
        NavMeshPath path = new NavMeshPath();
        if(_agent.CalculatePath(targetPoint, path))
        {
            pathData.IsReacheble = true;
            pathData.Path = path;

            pathData.Distance = 0;

            for (int i = 0; i < pathData.Path.corners.Length - 1; i++)
            {
                pathData.Distance += Vector3.Distance(pathData.Path.corners[i], pathData.Path.corners[i + 1]);
            }

            return true;
        }

        pathData.IsReacheble = true;
        return false;
    }

    bool moving = false;
    public void StartMove(PathData pathData)
    {
        _agent.SetPath(pathData.Path);
        moving = true;
    }

    private void Update()
    {

        if (moving && _agent.remainingDistance <= _agent.stoppingDistance + _pathEndThreshold)
        {
            moving = false;
            OnMoveComplete?.Invoke();
        }
        
        if(moving)
        {
            Debug.Log(Velocity.magnitude);
        }

        if (_agent.isOnOffMeshLink) Debug.Log("On link!");
    }
}

public struct PathData
{
    public NavMeshPath Path;
    public float Distance;
    public bool IsReacheble;
    //public TileCover Cover;
    //public Vector3 finalDirection;
}
