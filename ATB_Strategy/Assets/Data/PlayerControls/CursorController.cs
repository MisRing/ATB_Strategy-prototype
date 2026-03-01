using System;
using UnityEngine;
using System.Collections.Generic;

public class CursorController : MonoBehaviour
{
    [Header("Main settings")]
    [SerializeField] private TileCursor _tileCursor;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask _groundMasks;
    [SerializeField] private float _rayDistance = 100f;

    private PlayerInputController _playerInput;
    private GridMap _gridMap;

    private Vector3 _cursorPosition;
    public Vector3 CursorPosition { get => _cursorPosition; }

    public event Action OnPositionChanged;

    public void Init(GridMap gridMap, PlayerInputController playerInput)
    {
        _gridMap = gridMap;
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

        Vector3 tileWorldPos = Vector3.zero;

        bool cursorOnTile = false;

        if (Physics.Raycast(ray, out hit, _rayDistance))
        {
            if (((1 << hit.collider.gameObject.layer) & _groundMasks.value) != 0)
            {
                Vector3 realPoint = hit.point;

                if (_gridMap.HasTile(realPoint.x, realPoint.z))
                {
                    tileWorldPos = _gridMap.GetTileWorldPosition(realPoint.x, realPoint.z);
                    cursorOnTile = true;
                }
            }
        }

        if(cursorOnTile)
        {
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
