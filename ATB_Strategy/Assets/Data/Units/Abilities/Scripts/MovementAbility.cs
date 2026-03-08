using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MovementAbility : AbilityBasic, IPathHandler
{
    private PathData _pathData;
    [SerializeField] private float _accelerationDistance = 0.5f;
    [SerializeField] private float _minVelocity = 0.1f;
    [SerializeField] private float _rotationSpeed = 5f;
        
    public event Action<PathData> OnPathChanged;

    private void Awake()
    {
        AbilityViewRenderer.PathHandlers.Add(this);
    }

    public override void Init(UnitAbilityController abilityController)
    {
        base.Init(abilityController);
        AbilityName = "Simple movement";
    }

    public override void EnterPrepare(AbilityData abilityData)
    {
        base.EnterPrepare(abilityData);

        UpdateData(abilityData);
    }

    public override void ExitPrepare()
    {
        base.ExitPrepare();
    }

    public override void UpdateData(AbilityData abilityData)
    {
        if (_abilityController.Unit.State != UnitState.WaitingForOrder) return;

        base.UpdateData(abilityData);
        _pathData.IsReacheble = GridPathFinder.CalculatePath(ref _pathData, transform.position, _abilityData.TargetWorldPos);

        OnPathChanged?.Invoke(_pathData);
    }

    public override bool Execute()
    {
        if (!_pathData.IsReacheble) return false;

        _abilityController.Unit.UnitAgent.SetDestination(_pathData.Points[_pathData.Points.Count - 1]);
        //StartCoroutine(Move());

        PathData emptyPath = new PathData();
        emptyPath.IsReacheble = false;
        OnPathChanged?.Invoke(emptyPath);

        return base.Execute();
    }
    
    private IEnumerator Move()
    {
        float currentPassedDistance = 0f;
        int nextPointIndex = 1;
        float nextPointDistance = Vector3.Distance(_pathData.Points[0], _pathData.Points[1]);
        bool moveEnds = false;
        
        _abilityController.Unit.UnitAnimator.SetCover(_pathData.Cover != TileCover.None);

        while (currentPassedDistance < _pathData.Distance)
        {
            float velocity = GetVelocity(currentPassedDistance, _pathData.Distance);
            
            if (nextPointIndex == _pathData.Points.Count - 1 && !moveEnds && velocity < 1f)
            {
                moveEnds = true;
            }
            
            Vector3 direction = (_pathData.Points[nextPointIndex] - transform.position).normalized * velocity;

            float step = _abilityController.Unit.UnitStats.Speed * TimeService.TimeSpeedDelta;
            
            currentPassedDistance += step * velocity;
            
            MoveToDirection(direction, step);
            if (_pathData.Cover != TileCover.None && moveEnds)
            {
                RotateToDirection(_pathData.finalDirection);
            }
            else
            {
                RotateToDirection(direction);
            }
            
            if (currentPassedDistance >= nextPointDistance)
            {
                if(nextPointIndex + 1 >= _pathData.Points.Count) break;
                
                nextPointDistance += Vector3.Distance(
                    _pathData.Points[nextPointIndex],
                    _pathData.Points[nextPointIndex + 1]
                    );
                
                nextPointIndex++;
            }
            
            yield return null;
        }

        _abilityController.Unit.UnitAnimator.SetMovement(0f, 0f);

        _abilityController.FinishExecuteAbility();
    }

    private void MoveToDirection(Vector3 direction, float step)
    {
        transform.position += direction * step;
            
        _abilityController.Unit.UnitAnimator.SetMovement(direction.x, direction.z);
    }

    private void RotateToDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * TimeService.TimeSpeedDelta);
        }
    }

    private float GetVelocity(float passedDistance, float fullDistance)
    {
        float startVelocity = Mathf.Clamp01(passedDistance / _accelerationDistance);
        startVelocity = 1f - MathF.Pow(1f - startVelocity, 2);

        float endVelocity = Mathf.Clamp01((fullDistance - passedDistance) / _accelerationDistance);
        endVelocity = 1f - MathF.Pow(1f - endVelocity, 2);

        float velocity = Mathf.Min(startVelocity, endVelocity);

        return Mathf.Max(velocity, _minVelocity);
    }
}
