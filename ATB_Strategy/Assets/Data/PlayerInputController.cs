using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private InputActions _inputActions;

    public bool IsReverseModifier { get => _inputActions.Player.ReverseInputModifier.IsPressed(); }

    public Vector2 MouseScreenPosition { get => _inputActions.Player.MousePosition.ReadValue<Vector2>(); }


    public event Action SelectObject;
    public event Action SelectPoint;
    public event Action SwitchTarget;


    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();

        _inputActions.Player.SelectObject.started += SelectObjectInput;
        _inputActions.Player.SelectPoint.started += SelectPointInput;
        _inputActions.Player.SwitchTarget.started += SwitchTargetInput;
    }

    private void OnDisable()
    {
        _inputActions.Disable();

        _inputActions.Player.SelectObject.started -= SelectObjectInput;
        _inputActions.Player.SelectPoint.started -= SelectPointInput;
        _inputActions.Player.SwitchTarget.started -= SwitchTargetInput;
    }

    private void SelectObjectInput(InputAction.CallbackContext context) => SelectObject?.Invoke();

    private void SelectPointInput(InputAction.CallbackContext context) => SelectPoint?.Invoke();

    private void SwitchTargetInput(InputAction.CallbackContext context) => SwitchTarget?.Invoke();
}
