using UnityEngine;
using System.Collections.Generic;
using System;

//[Serializable]
//public struct GridTile
//{
//    public bool IsGround;
//    public bool IsEmpty;
//    public TileCover[] Covers;

//    public UnitController Owner;

//    public int PositionX;
//    public int PositionZ;
//    public int Floor;
//    public float DeltaY;
    
//    public Vector3 WorldPosition;
//}

[Serializable]
public class GridTileDemo
{
    public bool IsGround;
    public bool IsEmpty;
    public TileCover[] Covers;
    //[NonSerialized] public GridTileDemo[] Neighbours;

    [NonSerialized] public UnitController Owner;

    public readonly int PositionX;
    public readonly int PositionZ;
    public readonly int Floor;

    public Vector3 WorldPosition;

    public GridTileDemo(int x, int z, int floor)
    {
        PositionX = x;
        PositionZ = z;
        Floor = floor;
        Covers = new TileCover[4];
        //Neighbours = new GridTileDemo[4];
    }
}