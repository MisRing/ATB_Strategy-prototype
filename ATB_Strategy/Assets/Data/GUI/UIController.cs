using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private UIAbilityController _uiAbilityController;

    [SerializeField] private GameObject _pauseCanvas;

    private void Awake()
    {
        _uiAbilityController.Init(_playerController);
    }
    
    private void OnEnable()
    {
        _playerController.PlayerSelectionManager.OnSelectionChanged += _uiAbilityController.SetAbilityButtons;
        _playerController.OnAbilitySelected += _uiAbilityController.SetAbilityPreparePanel;


        PlayerInputController.Cancel.DefaultAction += OpenPauseMenu;
    }

    private void OnDisable()
    {
        _playerController.PlayerSelectionManager.OnSelectionChanged -= _uiAbilityController.SetAbilityButtons;
        _playerController.OnAbilitySelected += _uiAbilityController.SetAbilityPreparePanel;

        PlayerInputController.Cancel.DefaultAction -= OpenPauseMenu;
    }

    private bool _pauseOpened = false;
    private void OpenPauseMenu()
    {
        _pauseOpened = !_pauseOpened;
        _pauseCanvas.SetActive(_pauseOpened);
        TimeService.GamePause(_pauseOpened);
    }
}
