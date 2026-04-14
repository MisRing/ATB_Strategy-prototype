using UnityEngine;
using System;

public interface ICombat
{
    Vector3 Position { get; }

    public BodyPart[] BodyParts { get; }

    UnitOwner Owner { get; }

    [Serializable]
    public struct BodyPart
    {
        public Transform transform;
        [Range(0f,1f)] public float weight;
    }
}