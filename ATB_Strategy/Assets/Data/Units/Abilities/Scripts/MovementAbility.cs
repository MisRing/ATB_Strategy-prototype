using System;
using System.Collections;
using UnityEngine;

public class MovementAbility : AbilityBasic, IPathHandler
{
    private PathData _pathData;

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
        if (Status == AbilityStatus.Executing) return;

        base.UpdateData(abilityData);
        _pathData.IsReacheble = GridPathFinder.CalculatePath(ref _pathData, transform.position, _abilityData.TargetWorldPos);

        OnPathChanged?.Invoke(_pathData);
    }

    public override bool Execute()
    {
        if (!_pathData.IsReacheble) return false;
        //base.Execute();

        Debug.Log("Start executing <" + AbilityName + ">");
        Status = AbilityStatus.Executing;
        _abilityController.Unit.State = UnitState.Engaged;

        StartCoroutine(Move());
        return true;
    }

    private IEnumerator Move()
    {
        while (_pathData.Points.Count > 0)
        {
            Vector3 direction = (_pathData.Points[0] - transform.position).normalized;
            transform.position += direction * TimeService.TimeSpeedDelta * _abilityController.Unit.UnitStats.Speed;

            _abilityController.Unit.UnitAnimator.SetMovement(direction.x, direction.z);

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * TimeService.TimeSpeedDelta);
            }

            if (Vector3.Distance(transform.position, _pathData.Points[0]) <= 0.1f)
            {
                _pathData.Points.RemoveAt(0);
            }
            yield return null;
        }

        _abilityController.Unit.UnitAnimator.SetMovement(0f, 0f);

        Status = AbilityStatus.None;
        _abilityController.Unit.State = UnitState.WaitingForOrder;
    }
}
