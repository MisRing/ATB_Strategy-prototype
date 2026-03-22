using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitAgentController : MonoBehaviour
{
    [SerializeField] private float _pathEndThreshold = 0.05f;
    [SerializeField] private float _accelerationDistance = 0.5f;
    [SerializeField] private float _minVelocity = 0.1f;

    public int CurrentTileX;
    public int CurrentTileZ;
    public int CurrentTileFloor;

    public Vector3 Velocity { get { return _velocity; } }
    Vector3 _velocity = Vector3.zero;

    public PathData PathData { get { return _pathData; } }
    private PathData _pathData;
    
    private NavMeshAgent _agent;
    private UnitController _unit;

    public void Init(UnitController unit, GridTile startTile)
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.acceleration = 100000f;

        _unit = unit;
        WarpAgentToTile(startTile);

        _agent.speed = 0;
    }

    private void WarpAgentToTile(GridTile tile)
    {
        CurrentTileX = tile.PositionX;
        CurrentTileZ = tile.PositionZ;
        CurrentTileFloor = tile.Floor;
        
        _agent.Warp(GridParameters.LevelGrid.GetTileWorldPos(tile));
        GridParameters.LevelGrid.SetTileOwner(CurrentTileX, CurrentTileZ, CurrentTileFloor, _unit);
    }
    
    private void SetAgentTile(GridTile tile)
    {
        GridParameters.LevelGrid.SetTileOwner(CurrentTileX, CurrentTileZ, CurrentTileFloor, null);

        CurrentTileX = tile.PositionX;
        CurrentTileZ = tile.PositionZ;
        CurrentTileFloor = tile.Floor;
        
        GridParameters.LevelGrid.SetTileOwner(CurrentTileX, CurrentTileZ, CurrentTileFloor, _unit);
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

            float duration = CalculateDuration(_pathData.Distance, _unit.UnitStats.Speed);

            int turns = Mathf.CeilToInt(duration / TurnManager.TurnTime);

            _pathData.Duration = duration;
            _pathData.TurnsCost = turns;

            return;
        }

        _pathData.IsReacheble = false;
    }
    
    float CalculateDuration(float fullDistance, float normalSpeed, float timeStep = 0.05f)
    {
        float time = 0f;
        float passed = 0f;

        while (passed < fullDistance)
        {
            float remaining = fullDistance - passed;

            float velocity = GetVelocity(passed, remaining);

            // distance = speed * time → time = distance / speed
            float deltaTime = timeStep / velocity;
            float passedByStep = velocity * normalSpeed * timeStep;

            time += timeStep;
            passed += passedByStep;
        }

        return time;
    }

    public void StartMove()
    {
        _agent.SetPath(_pathData.Path);
        GridTile finalTile = new GridTile();
        GridParameters.LevelGrid.GetTileByWorldPos(ref finalTile,_pathData.Path.corners[_pathData.Path.corners.Length - 1]);
        SetAgentTile(finalTile);

        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (true)
        {
            SetAgentSpeed();

            if (_agent.remainingDistance <= _agent.stoppingDistance + _pathEndThreshold)
            {
                break;
            }
            yield return null;
        }
        
        EndMove();
    }

    private void SetAgentSpeed()
    {
        float passedDistance = _pathData.Distance - _agent.remainingDistance;
        if (_agent.remainingDistance == float.PositiveInfinity)
        {
            passedDistance = Vector3.Distance(_pathData.Path.corners[0], transform.position);
        }
        float acceleration = GetVelocity(passedDistance, _agent.remainingDistance);
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
        //if(velocity < _minVelocity) Debug.Log(startVelocity + " | " + endVelocity + " | " + velocity);
        return Mathf.Max(velocity, _minVelocity);
    }

    private void EndMove()
    {
        _velocity = Vector3.zero;
        _agent.speed = 0f;
        _agent.Warp(_agent.pathEndPosition);
        _agent.ResetPath();;
    }
}
public struct PathData
{
    public NavMeshPath Path;
    public int TurnsCost;
    public float Duration;
    public float Distance;
    public bool IsReacheble;
    //public TileCover Cover;
    //public Vector3 finalDirection;
}
