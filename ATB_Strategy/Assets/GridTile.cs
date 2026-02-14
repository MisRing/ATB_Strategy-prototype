using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public struct GridTile
{
    public bool IsGround;
    public bool IsEmpty;

    public int PositionX;
    public int PositionZ;
    public float DeltaY;
}
