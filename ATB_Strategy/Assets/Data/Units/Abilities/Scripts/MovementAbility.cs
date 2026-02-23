using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementAbility : IAbility
{
    public AbilityType Type {get;private set; }
    public AbilityStatus Status { get; private set; }

    private UnityAbilityController _abilityController;

    public void Init(UnityAbilityController abilityController)
    {
        Status = AbilityStatus.Idle;
        Type = AbilityType.Targeted;
        _abilityController = abilityController;
    }

    public void EnterPrepare()
    {
        Status = AbilityStatus.Preparing;
        CalculatePath();

        Debug.Log("Movement ability on prepare");
    }

    public void CalculatePath()
    {
        //if (targetData == null) return;

        //targetData.PathData = new PathData();
        //targetData.PathData.Points = new List<Vector3>();
        //if (targetData.TargetPoint != _abilityController.transform.position)
        //{
        //    targetData.PathData.IsReacheble = GridPathFinder.CalculatePath(ref targetData.PathData,
        //        _abilityController.transform.position, targetData.TargetPoint);
        //}
        //else
        //{
        //    targetData.PathData.IsReacheble = false;
        //}
    }

    public void Cancel()
    {
        Status = AbilityStatus.Idle;

        Debug.Log("Movement ability canceled");
    }

    public void Execute()
    {
        Status = AbilityStatus.Executing;

        Debug.Log("Movement ability executing");
    }
}
