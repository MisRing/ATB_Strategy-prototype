using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public struct GridTile
{
    public bool IsGround;
    public bool IsEmpty;
    public TileCover[] Covers;

    public int PositionX;
    public int PositionZ;
    public float DeltaY;

    public Vector3 GridOffset;
}

public enum TileCover
{
    None,
    Low,
    Full,
}
