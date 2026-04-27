using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private UIAbilityController _uiAbilityController;
    [SerializeField] private UIUnitTargets _unitTargets;
    [SerializeField] private UIUtilityPanel _utilityPanel;
    [SerializeField] private UISquadPanel _squadPanel;

    [SerializeField] private GameObject _pauseCanvas;

    private bool _pauseOpened = false;
    
    private void Awake()
    {
        _uiAbilityController.Init(_playerController);
        _unitTargets.Init(_playerController);
        _squadPanel.Init(_playerController.PlayerSelectionManager);
    }
    
    private void OnEnable()
    {
        _playerController.PlayerSelectionManager.OnSelectionChanged += _uiAbilityController.SetAbilityButtons;
        _playerController.PlayerSelectionManager.OnSelectionChanged += _unitTargets.SetUnitTargets;
        _playerController.PlayerSelectionManager.OnSelectionChanged += _utilityPanel.UnitSelected;
        _playerController.OnAbilitySelected += _uiAbilityController.SetAbilityPreparePanel;

        PlayerInputController.SelectUtility += _utilityPanel.OpenUtilityPanel;

        PlayerInputController.Cancel.DefaultAction += OpenPauseMenu;
    }

    private void OnDisable()
    {
        _playerController.PlayerSelectionManager.OnSelectionChanged -= _uiAbilityController.SetAbilityButtons;
        _playerController.PlayerSelectionManager.OnSelectionChanged -= _unitTargets.SetUnitTargets;
        _playerController.PlayerSelectionManager.OnSelectionChanged -= _utilityPanel.UnitSelected;
        _playerController.OnAbilitySelected += _uiAbilityController.SetAbilityPreparePanel;

        PlayerInputController.SelectUtility -= _utilityPanel.OpenUtilityPanel;

        PlayerInputController.Cancel.DefaultAction -= OpenPauseMenu;
    }

    private void OpenPauseMenu()
    {
        _pauseOpened = !_pauseOpened;
        _pauseCanvas.SetActive(_pauseOpened);
        TimeService.GamePause(_pauseOpened);

        if (_pauseOpened)
        {
            GameGlobalComandService.ResetPlayerCommands();
        }
    }
}
