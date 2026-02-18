using System;
using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnitAnimator))]
public class UnitComponent : MonoBehaviour
{
    public UnitStats UnitStats;
    public UnitAnimator UnitAnimator;

    public event Action<bool> OnSelectionChanged;

    private bool _isSelected;

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

    public void Select()
    {
        if (_isSelected) return;

        _isSelected = true;
        OnSelectionChanged?.Invoke(true);
    }

    public void Deselect()
    {
        if (!_isSelected) return;

        _isSelected = false;
        OnSelectionChanged?.Invoke(false);
    }

    public void MoveToTile(ref GridTile tile, GridMap grid)
    {
        _positionX = tile.PositionX;
        _positionZ = tile.PositionZ;

        transform.position = new Vector3(_positionX, tile.DeltaY, _positionZ) + grid.transform.position;


    }
}
