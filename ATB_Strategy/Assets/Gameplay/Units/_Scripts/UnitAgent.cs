using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitAgent : MonoBehaviour, IPathHandler
{
    [SerializeField] private float _accelerationDistance = 0.5f;
    [SerializeField] private float _minAcceleration = 0.1f;
    [SerializeField] private float _rotationSpeed = 36f;
    [SerializeField] private float _coverDistance = 2f;
    
    [SerializeField] private float _visibilityResetTriggerDistance = 2f;
    private int _visibilityResetTimer = 0;

    public Vector3Int CurrentTile;
    
    public Vector3 Velocity { get { return _velocity; } }
    private Vector3 _velocity = Vector3.zero;

    public PathData PathData { get { return _pathData; } }
    private PathData _pathData;
    private bool _showPath;
    private UnitController _unit;

    public event Action<PathData> OnPathChanged;
    public event Action<TileCover, int, float> OnCoverChanged;

    private void Awake()
    {
        AbilityViewRenderer.PathHandlers.Add(this);
    }

    public void Init(UnitController unit, GridTile startTile)
    {
        _unit = unit;
        WarpAgentToTile(startTile);
    }

    private void WarpAgentToTile(GridTile tile)
    {
        CurrentTile = new Vector3Int(tile.PositionX, tile.Floor, tile.PositionZ);
        
        transform.position = GridParameters.LevelGrid.GetTileWorldPos(tile);
        GridParameters.LevelGrid.SetTileOwner(CurrentTile.x, CurrentTile.z, CurrentTile.y, _unit);
    }
    
    private void SetAgentTile(GridTile tile)
    {
        GridParameters.LevelGrid.SetTileOwner(CurrentTile.x, CurrentTile.z, CurrentTile.y, null);

        CurrentTile = new Vector3Int(tile.PositionX, tile.Floor, tile.PositionZ);

        GridParameters.LevelGrid.SetTileOwner(CurrentTile.x, CurrentTile.z, CurrentTile.y, _unit);
    }

    public void RemoveFromGrid()
    {
        GridParameters.LevelGrid.SetTileOwner(CurrentTile.x, CurrentTile.z, CurrentTile.y, null);
        CurrentTile = Vector3Int.zero;
    }

    public void ShowPath(bool show)
    {
        _showPath = show;
        OnPathChanged?.Invoke(_showPath ? _pathData : null);
    }

    public void CalculatePath(Vector3 targetPosition)
    {
        _pathData = new PathData();
        if (GridPathFinder.CalculatePath(ref _pathData, transform.position, targetPosition))
        {
            _pathData.Duration = CalculateDuration(_pathData.Distance, _unit.UnitStats.Speed);

            float realTurnsCost = Mathf.Round((_pathData.Duration / TurnManager.TurnTime) * 100f) / 100f;
            
            _pathData.TurnsCost = Mathf.CeilToInt(realTurnsCost);
        }
        else
        {
            _pathData = null;
        }

        OnPathChanged?.Invoke(_showPath ? _pathData : null);
    }

    private float CalculateDuration(float fullDistance, float normalSpeed, float timeStep = 0.05f)
    {
        float time = 0f;
        float passed = 0f;

        while (passed < fullDistance)
        {
            float remaining = fullDistance - passed;

            float acceleration = GetAcceleration(passed, remaining);

            float passedByStep = acceleration * normalSpeed * timeStep;

            time += timeStep;
            passed += passedByStep;
        }

        return time;
    }

    public bool StartMove()
    {
        if (_pathData == null) return false;

        //?????????????????????????????
        GridTile finalTile = new GridTile();
        GridParameters.LevelGrid.GetTileByWorldPos(ref finalTile, _pathData.Points[_pathData.Points.Count - 1]);
        SetAgentTile(finalTile);
        //?????????????????????????????

        StartCoroutine(Move());
        return true;
    }

    private IEnumerator Move()
    {
        float currentPassedDistance = 0f;
        int nextPointIndex = 1;
        float nextPointDistance = Vector3.Distance(_pathData.Points[0], _pathData.Points[1]);

        bool coverSet = false;
        float distanceToCover = _coverDistance < _pathData.Distance ? _coverDistance : _pathData.Distance;

        while (currentPassedDistance < _pathData.Distance)
        {
            float remainingDistance = _pathData.Distance - currentPassedDistance;
            float velocity = GetAcceleration(currentPassedDistance, remainingDistance);

            Vector3 direction = (_pathData.Points[nextPointIndex] - transform.position).normalized * velocity;

            float step = _unit.UnitStats.Speed * TimeService.TimeSpeedDelta;

            currentPassedDistance += step * velocity;

            MoveToDirection(direction, step);
            if (remainingDistance <= distanceToCover)
            {
                if (!coverSet)
                {
                    GridTile finalTile = new GridTile();
                    GridParameters.LevelGrid.GetTileByWorldPos(ref finalTile, _pathData.Points[_pathData.Points.Count - 1]);
                    _pathData.Cover = GridPathFinder.GetTileCover(
                        ref _pathData.FinalDirection,
                        out _pathData.CoverLook,
                        finalTile,
                        _unit.UnitCombat.Targets
                        );

                    coverSet = true;
                }
                float percent = 1f - (remainingDistance / distanceToCover);
                OnCoverChanged?.Invoke(_pathData.Cover, _pathData.CoverLook, percent);

                LerpRotate(_pathData.FinalDirection, percent);
            }
            else
            {
                RotateToDirection(direction, velocity);
            }

            if (currentPassedDistance >= nextPointDistance)
            {
                if (nextPointIndex + 1 >= _pathData.Points.Count) break;

                nextPointDistance += Vector3.Distance(
                    _pathData.Points[nextPointIndex],
                    _pathData.Points[nextPointIndex + 1]
                    );

                nextPointIndex++;
            }

            if (_visibilityResetTimer >= currentPassedDistance / _visibilityResetTriggerDistance)
            {
                _visibilityResetTimer++;
                CombatService.TriggerVisibilityReset();
                CombatService.ResetVisibility();
            }

            yield return null;
        }

        EndMove();
    }

    private float GetAcceleration(float passedDistance, float remainingDistance)
    {
        float startVelocity = Mathf.Clamp01(passedDistance / _accelerationDistance);
        startVelocity = 1f - MathF.Pow(1f - startVelocity, 2);

        float endVelocity = Mathf.Clamp01(remainingDistance / _accelerationDistance);
        endVelocity = 1f - MathF.Pow(1f - endVelocity, 2);

        float velocity = Mathf.Min(startVelocity, endVelocity);
        
        return Mathf.Max(velocity, _minAcceleration);
    }

    private void MoveToDirection(Vector3 direction, float step)
    {
        transform.position += direction * step;

        _velocity = direction;
    }

    private void RotateToDirection(Vector3 direction, float velocity)
    {
        if (direction != Vector3.zero)
        {
            direction.y = 0;
            direction = direction.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * velocity * TimeService.TimeSpeedDelta);
        }
    }

    private void LerpRotate(Vector3 direction, float t)
    {
        direction = _pathData.FinalDirection;

        direction.y = 0;
        direction = direction.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
    }

    private void EndMove()
    {
        _velocity = Vector3.zero;
        transform.position = _pathData.Points.Last();
        _pathData = null;
    }
}

public class PathData
{
    public List<Vector3> Points;
    public int TurnsCost;
    public float Duration;
    public float Distance;
    public TileCover Cover;
    public int CoverLook;
    public Vector3 FinalDirection;
}
