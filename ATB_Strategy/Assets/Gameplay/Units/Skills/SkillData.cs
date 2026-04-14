using UnityEngine;

public abstract class SkillData { }

public class TargetData : SkillData
{
    public GameObject Target;
}

public class PointData : SkillData
{
    public Vector3 Position;
}