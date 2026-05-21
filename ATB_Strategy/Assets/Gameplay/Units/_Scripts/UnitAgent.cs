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

    public GridTile CurrentTile;
    
    public Vector3 Velocity { get { return _velocity; } }
    private Vector3 _velocity = Vector3.zero;

    public PathData PathData { get { return _pathData; } }
    [SerializeField] private PathData _pathData;
    private bool _showPath;
    private UnitController _unit;

    public event Action<PathData> OnPathChanged;
    public event Action<TileCover, int, float> OnCoverChanged;

    public bool IsMoving = false;

    public void Init(UnitController unit, GridTile startTile)
    {
        _unit = unit;
        CurrentTile = startTile;
    }

    private void OnEnable()
    {
        AbilityViewRenderer.PathHandlers.Add(this);
    }

    private void OnDisable()
    {
        AbilityViewRenderer.PathHandlers.Remove(this);
    }

    private void Start()
    {
        WarpAgentToTile(CurrentTile);
    }

    private void WarpAgentToTile(GridTile tile)
    {
        CurrentTile = tile;

        transform.position = CurrentTile.WorldPosition;
        CurrentTile.Owner = _unit;
    }
    
    private void SetAgentTile(GridTile tile)
    {
        if (CurrentTile != null)
        {
            CurrentTile.Owner = null;
        }

        CurrentTile = tile;

        CurrentTile.Owner = _unit;
    }

    public void RemoveFromGrid()
    {
        if (CurrentTile == null) return;

        CurrentTile.Owner = null;
        CurrentTile = null;
    }

    public void ShowPath(bool show)
    {
        _showPath = show;
        OnPathChanged?.Invoke(_showPath ? _pathData : null);
    }

    public void CalculatePath(Vector3 targetPosition)
    {
        if (GridPathFinder.CalculatePath(out _pathData, transform.position, targetPosition))
        {
            _pathData.Duration = CalculateDuration(_pathData.Distance, _unit.Stats.Speed);

            float realTurnsCost = Mathf.Round((_pathData.Duration / TurnManager.TurnTime) * 100f) / 100f;
            
            _pathData.TurnsCost = Mathf.CeilToInt(realTurnsCost);
        }

        OnPathChanged?.Invoke(_showPath ? _pathData : null);
    }

    public bool CheckPath(Vector3 targetPosition, out int cost)
    {
        PathData data = new PathData();
        if (GridPathFinder.CalculatePath(out data, transform.position, targetPosition))
        {
            data.Duration = CalculateDuration(data.Distance, _unit.Stats.Speed);

            float realTurnsCost = Mathf.Round((data.Duration / TurnManager.TurnTime) * 100f) / 100f;

            data.TurnsCost = Mathf.CeilToInt(realTurnsCost);
            cost = data.TurnsCost;
            return true;
        }

        cost = int.MaxValue;
        return false;
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

    private Coroutine movement;
    public bool StartMove(out int moveCost)
    {
        moveCost = 0;
        if (_pathData == null) return false;
        if (_pathData.TurnsCost <= 0)
        {
#if UNITY_EDITOR
            string dataErrorLog =
                $"From ({_pathData.Points[0]}), To ({_pathData.Points[^1]}), Points count ({_pathData.Points.Count})";
            Debug.LogWarning($"Path is invalid: [{_unit.name} - {_unit.Owner}; Path data: {dataErrorLog}");
#endif
            return false;
        }
        
        if (movement != null)
        {
            StopCoroutine(movement);
        }

        //?????????????????????????????
        GridTile finalTile = GridParameters.LevelGrid.GetTileByWorldPos(_pathData.Points[^1]);
        if (finalTile == null || finalTile.Owner != null)
        {
            return false;
        }
        SetAgentTile(finalTile);
        //?????????????????????????????

        moveCost = _pathData.TurnsCost;
        IsMoving = true;
        movement = StartCoroutine(Move(_pathData));
        return true;
    }
    private IEnumerator Move(PathData path)
    {
        float currentPassedDistance = 0f;
        int nextPointIndex = 1;
        float nextPointDistance = Vector3.Distance(path.Points[0], path.Points[1]);

        bool coverSet = false;
        float distanceToCover = _coverDistance < path.Distance ? _coverDistance : path.Distance;

        int visibilityResetTimer = 0;

        while (currentPassedDistance < path.Distance)
        {
            yield return null;
            if (!IsMoving) break;

            float remainingDistance = path.Distance - currentPassedDistance;
            float velocity = GetAcceleration(currentPassedDistance, remainingDistance);

            Vector3 direction = (path.Points[nextPointIndex] - transform.position).normalized * velocity;

            float step = _unit.Stats.Speed * TimeService.TimeSpeedDelta;

            currentPassedDistance += step * velocity;

            MoveToDirection(direction, step);
            if (remainingDistance <= distanceToCover)
            {
                if (!coverSet)
                {
                    GridTile finalTile = GridParameters.LevelGrid.GetTileByWorldPos(path.Points[^1]);
                    path.Cover = GridPathFinder.GetTileCover(
                        ref path.FinalDirection,
                        out path.CoverLook,
                        finalTile,
                        _unit.Combat.Targets
                        );

                    coverSet = true;
                }
                float percent = 1f - (remainingDistance / distanceToCover);
                OnCoverChanged?.Invoke(path.Cover, path.CoverLook, percent);

                LerpRotate(path.FinalDirection, percent);
            }
            else
            {
                RotateToDirection(direction, velocity);
            }

            if (currentPassedDistance >= nextPointDistance)
            {
                if (nextPointIndex + 1 >= path.Points.Count) break;

                nextPointDistance += Vector3.Distance(
                    path.Points[nextPointIndex],
                    path.Points[nextPointIndex + 1]
                    );

                nextPointIndex++;
            }

            if (visibilityResetTimer <= currentPassedDistance / _visibilityResetTriggerDistance)
            {
                visibilityResetTimer++;
                FogOfWarUtility.TriggerVisibilityReset();
                FogOfWarUtility.ResetVisibility();
            }
        }

    }

    public void EndMove()
    {
        IsMoving = false;
        _velocity = Vector3.zero;
        transform.position = _pathData.Points.Last();
        _pathData = null;
    }

    public void InterruptMovement()
    {
        if (movement != null)
        {
            StopCoroutine(movement);
            movement = null;
        }
        IsMoving = false;
        _velocity = Vector3.zero;
        _pathData = null;
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

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _rotationSpeed * velocity * TimeService.TimeSpeedDelta
                );
        }
    }

    private void LerpRotate(Vector3 direction, float t)
    {
        direction.y = 0;
        direction = direction.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
    }
}

[Serializable]
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
