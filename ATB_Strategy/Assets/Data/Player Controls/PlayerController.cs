using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public CameraController CameraController;
    [HideInInspector] public CursorController CursorController;
    [HideInInspector] public PlayerSelectionManager PlayerSelectionManager;

    [SerializeField] private List<Vector3Int> _positionPreset = new List<Vector3Int>();

    private void Awake()
    {
        if (GridParameters.LevelGrid == null)
        {
            GridParameters.LevelGrid = FindFirstObjectByType(typeof(GridMap)) as GridMap;
        }

        CursorController = GetComponent<CursorController>();
        PlayerSelectionManager = GetComponent<PlayerSelectionManager>();
        
        PlayerSelectionManager.Init(this, _positionPreset);

        CursorController.Init();
        CameraController.Init(PlayerSelectionManager.SelectedUnit.transform);
    }

    private void OnEnable()
    {
        PlayerInputController.SelectAbility += SelectAbility;
        PlayerInputController.SelectPoint += SelectPoint;

        CursorController.OnPositionChanged += UpdateAbilityData;

        PlayerSelectionManager.OnSelectionChanged += UnitSelected;
    }

    private void OnDisable()
    {
        PlayerInputController.SelectAbility -= SelectAbility;
        PlayerInputController.SelectPoint -= SelectPoint;

        CursorController.OnPositionChanged -= UpdateAbilityData;
        
        PlayerSelectionManager.OnSelectionChanged -= UnitSelected;
    }
    
    private void SelectPoint()
    {
        if (!PlayerSelectionManager.SelectedUnit || PlayerSelectionManager.SelectedUnit.State == UnitState.Engaged) return;

        if (PlayerSelectionManager.SelectedUnit.AbilityController.ExecuteAbility())
        {
            PlayerSelectionManager.SwitchToFreeUnit();
        }
    }

    private void UpdateAbilityData()
    {
        if (!PlayerSelectionManager.SelectedUnit || PlayerSelectionManager.SelectedUnit.State == UnitState.Engaged) return;

        AbilityData data = new AbilityData();
        data.TargetWorldPos = CursorController.CursorPosition;
        PlayerSelectionManager.SelectedUnit.AbilityController.UpdateAbilityData(data);
    }

    private void SelectAbility(int index)
    {
        if (!PlayerSelectionManager.SelectedUnit) return;

        if(PlayerSelectionManager.SelectedUnit.AbilityController.SelectAbility(index))
        {
            SelectPoint();
            return;
        }

        UpdateAbilityData();
    }

    private void UnitSelected(UnitController unit)
    {
        UpdateAbilityData();
    }

    
    //---------------------------------------------DEBUG-GUI--------------------------------------------
    private void OnDrawGizmos()
    {
        if(GridParameters.LevelGrid == null)
        {
            GridParameters.LevelGrid = FindFirstObjectByType(typeof(GridMap)) as GridMap;
        }

        foreach(Vector3Int point in _positionPreset)
        {
            if(GridParameters.LevelGrid.CheckTile(point.x, point.z, point.y))
            {
                Gizmos.color = Color.darkGreen;
                Gizmos.DrawSphere(GridParameters.LevelGrid.GetTileWorldPos(point.x, point.z, point.y), 0.3f);
            }
        }
    }
}