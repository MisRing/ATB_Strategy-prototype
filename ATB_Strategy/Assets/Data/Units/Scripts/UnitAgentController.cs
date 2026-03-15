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
    [SerializeField] private float _accelerationDistance = 0.5f;
    [SerializeField] private float _minVelocity = 0.1f;

    public Vector3 Velocity { get { return _velocity; } }
    Vector3 _velocity = Vector3.zero;

    public PathData PathData { get { return _pathData; } }
    private PathData _pathData;

    private bool _isMoving = false;

    private NavMeshAgent _agent;
    private UnitController _unit;

    public event Action OnMoveComplete;

    public void Init(UnitController unit, GridTile startTile)
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.acceleration = 100000f;

        _unit = unit;
        _agent.Warp(GridParameters.LevelGrid.GetTileWorldPos(startTile));

        _agent.speed = 0;
    }

    public void CalculatePath(Vector3 targetPoint)
    {
        _pathData = new PathData();
        NavMeshPath path = new NavMeshPath();
        if(_agent.CalculatePath(targetPoint, path))
        {
            _pathData.IsReacheble = true;
            _pathData.Path = path;

            _pathData.Distance = 0;

            for (int i = 0; i < _pathData.Path.corners.Length - 1; i++)
            {
                _pathData.Distance += Vector3.Distance(_pathData.Path.corners[i], _pathData.Path.corners[i + 1]);
            }

            return;
        }

        _pathData.IsReacheble = false;
    }

    float time = 0f;
    public void StartMove()
    {
        _agent.SetPath(_pathData.Path);
        _isMoving = true;
        time = Time.time;
    }

    private void Update()
    {
        if (_isMoving)
        {
            Move();
        }

        if (_agent.isOnOffMeshLink) Debug.Log("On link!");
    }

    private void Move()
    {
        SetAgentSpeed();

        if (_agent.remainingDistance <= _agent.stoppingDistance + _pathEndThreshold)
        {
            EndMove();
        }
    }

    private void SetAgentSpeed()
    {
        float acceleration = GetVelocity(Vector3.Distance(_pathData.Path.corners[0], transform.position), _agent.remainingDistance);
        float currentSpeed = _unit.UnitStats.Speed * acceleration * TimeService.TimeSpeed;
        _agent.speed = currentSpeed;

        if (_agent.velocity != Vector3.zero)
        {
            _velocity = _agent.velocity.normalized * acceleration;
        }
    }

    private float GetVelocity(float passedDistance, float remainingDistance)
    {
        float startVelocity = Mathf.Clamp01(passedDistance / _accelerationDistance);
        startVelocity = 1f - MathF.Pow(1f - startVelocity, 2);

        float endVelocity = Mathf.Clamp01(remainingDistance / _accelerationDistance);
        endVelocity = 1f - MathF.Pow(1f - endVelocity, 2);

        float velocity = Mathf.Min(startVelocity, endVelocity);

        return Mathf.Max(velocity, _minVelocity);
    }

    private void EndMove()
    {
        _velocity = Vector3.zero;
        _isMoving = false;
        _agent.Warp(_agent.pathEndPosition);
        OnMoveComplete?.Invoke();
        //Debug.Log(Time.time - time);
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
