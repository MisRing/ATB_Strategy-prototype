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
    public int Floor;
    public float DeltaY;
}
