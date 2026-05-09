using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorController : MonoBehaviour
{
    [Header("Main settings")]
    [SerializeField] private TileCursor _tileCursor;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask _groundMasks;
    [SerializeField] private float _rayDistance = 100f;
    [SerializeField] private float _maxRayNormalAngle = 60f;

    private GridTile _cursorTile;
    private Vector3 _cursorPosition;
    public Vector3 CursorPosition { get => _cursorPosition; }

    public event Action OnPositionChanged;

    public void Init()
    {
        _tileCursor.Init();
    }

    private void Update()
    {
        CalculatePosition();
    }

    private void CalculatePosition()
    {
        Vector2 mousePosition = PlayerInputController.MouseScreenPosition;

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        bool cursorOnTile = false;

        GridTile tile = null;

        if (!RaycastExtensions.IsPointerOverUIObject())
        {
            if (Physics.Raycast(ray, out hit, _rayDistance))
            {
                if (((1 << hit.collider.gameObject.layer) & _groundMasks.value) != 0)
                {
                    Vector3 realPoint = hit.point;
                    float normalAngle = Vector3.Angle(hit.normal, Vector3.up);
                    if (normalAngle <= _maxRayNormalAngle)
                    {
                        tile = GridParameters.LevelGrid.GetTileByWorldPos(realPoint);
                        if (tile != null && !tile.Owner)
                        {
                            cursorOnTile = true;
                        }
                    }
                }
            }
        }

        if(cursorOnTile)
        {
            UpdateCursorPosition(tile);
        }
        else
        {
            DisableAll();
        }
    }

    private void UpdateCursorPosition(GridTile tile)
    {

        if (_cursorTile != tile)
        {
            _cursorTile = tile;
            _cursorPosition = _cursorTile.WorldPosition;
            _tileCursor.SetTileCursor(_cursorTile);
            OnPositionChanged?.Invoke();
        }
    }

    private void DisableAll()
    {
        _cursorPosition = Vector3.up * 999f;
        _tileCursor.UnsetTileCursor();
        OnPositionChanged?.Invoke();
    }
}
