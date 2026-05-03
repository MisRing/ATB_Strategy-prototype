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

    public Vector3Int CurrentTile { get { return _currentTile; } }
    private Vector3Int _currentTile;
    
    public Vector3 Velocity { get { return _velocity; } }
    private Vector3 _velocity = Vector3.zero;

    //public PathData PathData { get { return _pathData; } }
    [SerializeField] private PathData _pathData;
    private bool _showPath;
    private UnitController _unit;

    public event Action<PathData> OnPathChanged;
    public event Action<TileCover, int, float> OnCoverChanged;

    public bool IsMoving = false;

    public void Init(UnitController unit, GridTile startTile)
    {
        _unit = unit;
        _currentTile = new Vector3Int(startTile.PositionX, startTile.Floor, startTile.PositionZ);
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
        WarpAgentToTile(_currentTile);
    }

    private void WarpAgentToTile(Vector3Int tilePos)
    {
        _currentTile = tilePos;

        transform.position = GridParameters.LevelGrid.GetTileWorldPos(_currentTile.x, _currentTile.z, _currentTile.y);
        GridParameters.LevelGrid.SetTileOwner(_currentTile.x, _currentTile.z, _currentTile.y, _unit);
    }
    
    private void SetAgentTile(GridTile tile)
    {
        GridParameters.LevelGrid.SetTileOwner(_currentTile.x, _currentTile.z, _currentTile.y, null);

        _currentTile = new Vector3Int(tile.PositionX, tile.Floor, tile.PositionZ);

        GridParameters.LevelGrid.SetTileOwner(_currentTile.x, _currentTile.z, _currentTile.y, _unit);
    }

    public void RemoveFromGrid()
    {
        GridParameters.LevelGrid.SetTileOwner(_currentTile.x, _currentTile.z, _currentTile.y, null);
        _currentTile = Vector3Int.zero;
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

    public bool StartMove(out int moveCost)
    {
        moveCost = 0;
        if (_pathData == null) return false;
        if (_pathData.TurnsCost <= 0)
        {
            string dataErrorLog =
                $"From ({_pathData.Points[0]}), To ({_pathData.Points[^1]}), Points count ({_pathData.Points.Count})";
            Debug.LogWarning($"Path is invalid: [{_unit.name} - {_unit.Owner}; Path data: {dataErrorLog}");
            return false;
        }

        //?????????????????????????????
        GridTile finalTile = new GridTile();
        if (!GridParameters.LevelGrid.GetTileByWorldPos(ref finalTile, _pathData.Points[_pathData.Points.Count - 1]) || finalTile.Owner != null)
        {
            return false;
        }
        SetAgentTile(finalTile);
        //?????????????????????????????

        moveCost = _pathData.TurnsCost;
        IsMoving = true;
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

        int visibilityResetTimer = 0;

        while (currentPassedDistance < _pathData.Distance)
        {
            if (!IsMoving) break;
            yield return null;

            float remainingDistance = _pathData.Distance - currentPassedDistance;
            float velocity = GetAcceleration(currentPassedDistance, remainingDistance);

            Vector3 direction = (_pathData.Points[nextPointIndex] - transform.position).normalized * velocity;

            float step = _unit.Stats.Speed * TimeService.TimeSpeedDelta;

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
                        _unit.Combat.Targets
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

            if (visibilityResetTimer >= currentPassedDistance / _visibilityResetTriggerDistance)
            {
                visibilityResetTimer++;
                CombatManager.TriggerVisibilityReset();
                CombatManager.ResetVisibility();
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
