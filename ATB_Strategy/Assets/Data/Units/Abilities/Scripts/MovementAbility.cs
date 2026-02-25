using System;
using UnityEngine;

public class MovementAbility : AbilityBasic, IPathHandler
{
    private PathData _pathData;

    public event Action<PathData> OnPathChanged;

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

    public override void Execute()
    {
        if (!_pathData.IsReacheble) return;
        base.Execute();
    }

    public override void UpdateData(AbilityData abilityData)
    {
        base.UpdateData(abilityData);
        _pathData.IsReacheble = GridPathFinder.CalculatePath(ref _pathData, transform.position, _abilityData.TargetWorldPos);

        OnPathChanged?.Invoke(_pathData);
    }

    private void Update() //prototype
    {
        if (Status != AbilityStatus.Executing) return;

        transform.position = Vector3.Lerp(transform.position, _pathData.Points[0], Time.deltaTime * 10f);

        if(Vector3.Distance(transform.position, _pathData.Points[0]) <= 0.1f)
        {
            _pathData.Points.RemoveAt(0);
        }

        if(_pathData.Points.Count <= 0)
        {
            Status = AbilityStatus.None;
        }
    }
}
