using System;
using UnityEngine;

public static class GridParameters
{
    public static GridMap LevelGrid;

    public static readonly float TILE_SIZE = 1f;
    public static readonly float LEVEL_HEIGHT = 2f;

    public static readonly LayerMask ENVIRONMENT_MASK = LayerMask.GetMask("Grid Environment");

    public static readonly Vector3[] COVER_DIRECTIONS = { Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.right };
    public static readonly float LOW_COVER_HEIGHT = 0.6f;
    public static readonly float FULL_COVER_HEIGHT = 1.25f;
}

public enum TileCover
{
    None,
    Low,
    Full,
}
