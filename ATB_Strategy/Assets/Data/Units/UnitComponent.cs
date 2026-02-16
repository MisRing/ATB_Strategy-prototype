using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitAnimator))]
public class UnitComponent : MonoBehaviour
{
    public UnitStats UnitStats;
    public UnitAnimator UnitAnimator;

    private int _positionX = -1;
    private int _positionZ = -1;

    private void Awake()
    {
        UnitStats = GetComponent<UnitStats>();
        UnitAnimator = GetComponent<UnitAnimator>();
    }

    public void Init(GridTile tile)
    {
        transform.position = new Vector3(tile.PositionX, tile.DeltaY, tile.PositionZ) + tile.GridOffset;
    }

    public void MoveToTile(ref GridTile tile, GridMap grid)
    {
        _positionX = tile.PositionX;
        _positionZ = tile.PositionZ;

        transform.position = new Vector3(_positionX, tile.DeltaY, _positionZ) + grid.transform.position;


    }
}
