using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitAgentController : MonoBehaviour
{
    [SerializeField] private float _accelerationDistance = 0.5f;
    [SerializeField] private float _minVelocity = 0.1f;
    [SerializeField] private float _rotationSpeed = 5f;

    private NavMeshAgent _agent;
    private UnitController _unit;

    public event Action OnMoveComplete;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.autoRepath = false;
    }

    public void Init(UnitController unit, GridTile startTile)
    {
        _unit = unit;
        _agent.Warp(GridParameters.LevelGrid.GetTileWorldPos(startTile));

        _agent.speed = _unit.UnitStats.Speed;
    }

    public bool CalculatePath(ref PathData pathData, Vector3 targetPoint)
    {
        NavMeshPath path = new NavMeshPath();
        if(_agent.CalculatePath(targetPoint, path))
        {
            pathData.IsReacheble = true;
            pathData.Points = path.corners.ToList();

            float lastDist = Vector3.Distance(pathData.Points[pathData.Points.Count - 2], pathData.Points[pathData.Points.Count - 1]);
            if (lastDist >= 3f)
            {
                Vector3 additionalPoint;
                float additionalPointDistPercent = (lastDist - 3) / lastDist;
                additionalPoint = (pathData.Points[pathData.Points.Count - 1] - pathData.Points[pathData.Points.Count - 2])
                    * additionalPointDistPercent + pathData.Points[pathData.Points.Count - 2];

                pathData.Points.Insert(pathData.Points.Count - 1, additionalPoint);
            }

            pathData.Distance = 0;

            for (int i = 0; i < pathData.Points.Count - 1; i++)
            {
                pathData.Distance += Vector3.Distance(pathData.Points[i], pathData.Points[i + 1]);
            }

            return true;
        }

        pathData.IsReacheble = true;
        return false;
    }

    public void StartMove(PathData pathData)
    {
        StartCoroutine(Move(pathData));
    }

    private IEnumerator Move(PathData pathData)
    {
        float currentPassedDistance = 0f;
        int nextPointIndex = 1;
        float nextPointDistance = Vector3.Distance(pathData.Points[0], pathData.Points[1]);
        bool moveEnds = false;

        //_abilityController.Unit.UnitAnimator.SetCover(_pathData.Cover != TileCover.None);

        while (currentPassedDistance < pathData.Distance)
        {
            float velocity = GetVelocity(currentPassedDistance, pathData.Distance);

            if (nextPointIndex == pathData.Points.Count - 1 && !moveEnds && velocity < 1f)
            {
                moveEnds = true;
            }

            Vector3 direction = (pathData.Points[nextPointIndex] - transform.position).normalized * velocity;

            float step = _unit.UnitStats.Speed * TimeService.TimeSpeedDelta;

            currentPassedDistance += step * velocity;

            MoveToDirection(direction, step);
            //if (pathData.Cover != TileCover.None && moveEnds)
            //{
            //    RotateToDirection(pathData.finalDirection);
            //}
            //else
            //{
                RotateToDirection(direction);
            //}

            if (currentPassedDistance >= nextPointDistance)
            {
                if (nextPointIndex + 1 >= pathData.Points.Count) break;

                nextPointDistance += Vector3.Distance(
                    pathData.Points[nextPointIndex],
                    pathData.Points[nextPointIndex + 1]
                    );

                nextPointIndex++;
            }

            yield return null;
        }

        //_abilityController.Unit.UnitAnimator.SetMovement(0f, 0f);

        OnMoveComplete?.Invoke();
    }

    private void MoveToDirection(Vector3 direction, float step)
    {
        transform.position += direction * step;

        //_abilityController.Unit.UnitAnimator.SetMovement(direction.x, direction.z);
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

public struct PathData
{
    public List<Vector3> Points;
    public float Distance;
    public bool IsReacheble;
    //public TileCover Cover;
    //public Vector3 finalDirection;
}
