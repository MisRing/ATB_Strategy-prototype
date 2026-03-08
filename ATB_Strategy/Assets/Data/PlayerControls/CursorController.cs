using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CursorController : MonoBehaviour
{
    [Header("Main settings")]
    [SerializeField] private TileCursor _tileCursor;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask _groundMasks;
    [SerializeField] private float _rayDistance = 100f;

    private PlayerInputController _playerInput;

    private GridTile _cursorTile;
    public GridTile CursorTile { get => _cursorTile; }

    private Vector3 _cursorPosition;
    public Vector3 CursorPosition { get => _cursorPosition; }

    public event Action OnPositionChanged;

    public void Init(PlayerInputController playerInput)
    {
        _playerInput = playerInput;
        _tileCursor.Init();
    }

    private void Update()
    {
        CalculatePosition();
    }

    private void CalculatePosition()
    {
        Vector2 mousePosition = _playerInput.MouseScreenPosition;

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        bool cursorOnTile = false;

        GridTile tile = new GridTile();

        if (Physics.Raycast(ray, out hit, _rayDistance))
        {
            if (((1 << hit.collider.gameObject.layer) & _groundMasks.value) != 0)
            {
                Vector3 realPoint = hit.point;

                if (GridParameters.LevelGrid.GetTileByWorldPos(ref tile, realPoint))
                {
                    cursorOnTile = true;
                }
            }
        }

        if(cursorOnTile)
        {
            _cursorTile = tile;
            Vector3 tileWorldPos = GridParameters.LevelGrid.GetTileWorldPos(tile);
            UpdateCursorPosition(tileWorldPos);
        }
        else
        {
            DisableAll();
        }
    }

    private void UpdateCursorPosition(Vector3 tileWorldPos)
    {
        if (_cursorPosition != tileWorldPos)
        {
            _cursorPosition = tileWorldPos;
            _tileCursor.SetTileCursor(_cursorPosition);
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
