using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private UIAbilityController _uiAbilityController;

    private void Start()
    {
        _uiAbilityController.SetAbilityButtons(_playerController.PlayerSelectionManager.SelectedUnit);
    }
    private void OnEnable()
    {
        _playerController.PlayerSelectionManager.OnSelectionChanged += _uiAbilityController.SetAbilityButtons;
    }

    private void OnDisable()
    {
        _playerController.PlayerSelectionManager.OnSelectionChanged -= _uiAbilityController.SetAbilityButtons;
    }
}
