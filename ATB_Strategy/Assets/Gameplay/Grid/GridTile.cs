using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class GridTile
{
    public bool IsGround;
    public bool IsEmpty;
    public TileCover[] Covers;

    [NonSerialized] public UnitController Owner;

    public int PositionX;
    public int PositionZ;
    public int Floor;

    public Vector3 WorldPosition;
    
    public TileVisibility[] Visibility = new TileVisibility[5];

    public GridTile(int x, int z, int floor)
    {
        PositionX = x;
        PositionZ = z;
        Floor = floor;
        Covers = new TileCover[4];
    }
}

public enum TileVisibility
{
    Hidden,
    Explored,
    Visible
}