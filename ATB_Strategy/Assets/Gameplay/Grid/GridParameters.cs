using UnityEngine;

public static class GridParameters
{
    public static GridMap LevelGrid;

    public static readonly float TILE_SIZE = 1f;
    public static readonly float LEVEL_HEIGHT = 2f;

    public static readonly LayerMask ENVIRONMENT_MASK = LayerMask.GetMask("Grid Environment");

    public static readonly Vector3[] COVER_DIRECTIONS = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
    public static readonly float LOW_COVER_HEIGHT = 0.6f;
    public static readonly float FULL_COVER_HEIGHT = 1.25f;
    
    public static readonly Vector3Int[] TILE_SIDES =
    {
        new Vector3Int( 1,  0,  0),
        new Vector3Int(-1,  0,  0),

        new Vector3Int( 0,  0,  1),
        new Vector3Int( 0,  0, -1),

        new Vector3Int( 0,  1,  0),
        new Vector3Int( 0, -1,  0),
    };
}

public enum TileCover
{
    None,
    Low,
    Full,
}
