using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public CameraController CameraController;
    [HideInInspector] public CursorController CursorController;
    [HideInInspector] public PlayerSelectionManager PlayerSelectionManager;

    [SerializeField] private List<Vector3Int> _positionPreset = new List<Vector3Int>();
    
    public event Action<int> OnAbilitySelected;

    private void Awake()
    {
        if (GridParameters.LevelGrid == null)
        {
            // TODO: DELETE THIS
            GridParameters.LevelGrid = FindFirstObjectByType(typeof(GridMap)) as GridMap; 
        }

        CursorController = GetComponent<CursorController>();
        PlayerSelectionManager = GetComponent<PlayerSelectionManager>();
        
        PlayerSelectionManager.Init(this, _positionPreset);

        CursorController.Init();
    }

    private void OnEnable()
    {
        PlayerInputController.SelectAbility += SelectAbility;

        PlayerSelectionManager.OnSelectionChanged += UnitSelected;
    }

    private void OnDisable()
    {
        PlayerInputController.SelectAbility -= SelectAbility;
        
        PlayerSelectionManager.OnSelectionChanged -= UnitSelected;
    }

    public void SelectAbility(int index)
    {
        if (!PlayerSelectionManager.SelectedUnit) return;
        
        AbilityBasic oldAbility = PlayerSelectionManager.SelectedUnit.AbilityController.CurrentAbility;
        if (PlayerSelectionManager.SelectedUnit.AbilityController.SelectAbility(index))
        {
            BindAbility(oldAbility, false);
            
            AbilityBasic newAbility = PlayerSelectionManager.SelectedUnit.AbilityController.CurrentAbility;
            
            bool isSameAbility = oldAbility == newAbility;
            bool isInstant = newAbility.RequiredDataType == null;

            if (isSameAbility && isInstant)
            {
                ExecuteAbility();
                return;
            }
            BindAbility(newAbility, true, index != -1);
            OnAbilitySelected?.Invoke(index);
        }
    }

    private void CancelAbility()
    {
        SelectAbility(-1);
    }

    private void UpdateAbilityPointData()
    {
        if (!PlayerSelectionManager.SelectedUnit || PlayerSelectionManager.SelectedUnit.State == UnitState.Engaged) return;

        PointData data = new PointData();
        data.Position = CursorController.CursorPosition;
        PlayerSelectionManager.SelectedUnit.AbilityController.UpdateAbilityData(data);
    }

    public void ExecuteAbility()
    {
        if (PlayerSelectionManager.SelectedUnit.AbilityController.ExecuteAbility())
        {
            OnAbilitySelected?.Invoke(-1);
            PlayerSelectionManager.SwitchToFreeUnit(true);
        }
    }


    private void BindAbility(AbilityBasic ability, bool bind, bool bindCancel = true)
    {
        if (!ability) return;

        Type type = ability.RequiredDataType;

        if (type == typeof(PointData))
        {
            if (bind)
            {
                CursorController.OnPositionChanged += UpdateAbilityPointData;
                PlayerInputController.PointRClick += ExecuteAbility;
                UpdateAbilityPointData();
            }
            else
            {
                CursorController.OnPositionChanged -= UpdateAbilityPointData;
                PlayerInputController.PointRClick -= ExecuteAbility;
            }
        }

        if (bind && bindCancel)
            PlayerInputController.Cancel += CancelAbility;
        else
            PlayerInputController.Cancel -= CancelAbility;
    }
    
    private void UnitSelected(UnitController oldUnit, UnitController unit)
    {
        if (oldUnit)
        {
            BindAbility(oldUnit.AbilityController.CurrentAbility, false);
        }
        if (unit)
        {
            SelectAbility(-1);
        }
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