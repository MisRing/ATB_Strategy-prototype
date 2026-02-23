using System;
using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(UnityAbilityController))]
[RequireComponent(typeof(UnitAnimator))]
public class UnitComponent : MonoBehaviour
{
    [HideInInspector] public UnitStats UnitStats;
    [HideInInspector] public UnityAbilityController AbilityController;
    [HideInInspector] public UnitAnimator UnitAnimator;

    public event Action<bool> OnSelectionChanged;

    private bool _isSelected;

    //private int _positionX = -1;
    //private int _positionZ = -1;

    public void Init(GridTile tile)
    {
        UnitStats = GetComponent<UnitStats>();
        AbilityController = GetComponent<UnityAbilityController>();
        UnitAnimator = GetComponent<UnitAnimator>();

        transform.position = new Vector3(tile.PositionX, tile.DeltaY, tile.PositionZ) + tile.GridOffset;

        AbilityController.Init(this);
    }

    public void Select()
    {
        if (_isSelected) return;

        _isSelected = true;
        OnSelectionChanged?.Invoke(true);

        AbilityController.SelectAbility(0);
    }

    public void Deselect()
    {
        if (!_isSelected) return;

        _isSelected = false;
        OnSelectionChanged?.Invoke(false);

        AbilityController.DeselectAbility();
    }
}
