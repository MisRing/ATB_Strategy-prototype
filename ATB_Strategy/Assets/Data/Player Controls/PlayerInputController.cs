using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private static InputActions _inputActions;

    public static bool IsReverseModifier { get => _inputActions.Player.ReverseInputModifier.IsPressed(); }

    public static Vector2 MouseScreenPosition { get => _inputActions.Player.MousePosition.ReadValue<Vector2>(); }


    public static event Action SelectObject;
    public static event Action SelectPoint;
    public static event Action SwitchTarget;

    public static event Action<int> SelectAbility;
    
    public static StackAction CancelESC = new StackAction();
    
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

        _inputActions.Player.AbilitySwitch0.started += SelectAbilityInput0;
        _inputActions.Player.AbilitySwitch1.started += SelectAbilityInput1;
        _inputActions.Player.AbilitySwitch2.started += SelectAbilityInput2;
        _inputActions.Player.AbilitySwitch3.started += SelectAbilityInput3;
        _inputActions.Player.AbilitySwitch4.started += SelectAbilityInput4;
        _inputActions.Player.AbilitySwitch5.started += SelectAbilityInput5;
        _inputActions.Player.AbilitySwitch6.started += SelectAbilityInput6;
        _inputActions.Player.AbilitySwitch7.started += SelectAbilityInput7;
        _inputActions.Player.AbilitySwitch8.started += SelectAbilityInput8;
        _inputActions.Player.AbilitySwitch9.started += SelectAbilityInput9;
        
        _inputActions.Player.Escape.started += CancelEscInput;
    }

    private void OnDisable()
    {
        _inputActions.Disable();

        _inputActions.Player.SelectObject.started -= SelectObjectInput;
        _inputActions.Player.SelectPoint.started -= SelectPointInput;
        _inputActions.Player.SwitchTarget.started -= SwitchTargetInput;

        _inputActions.Player.AbilitySwitch0.started -= SelectAbilityInput0;
        _inputActions.Player.AbilitySwitch1.started -= SelectAbilityInput1;
        _inputActions.Player.AbilitySwitch2.started -= SelectAbilityInput2;
        _inputActions.Player.AbilitySwitch3.started -= SelectAbilityInput3;
        _inputActions.Player.AbilitySwitch4.started -= SelectAbilityInput4;
        _inputActions.Player.AbilitySwitch5.started -= SelectAbilityInput5;
        _inputActions.Player.AbilitySwitch6.started -= SelectAbilityInput6;
        _inputActions.Player.AbilitySwitch7.started -= SelectAbilityInput7;
        _inputActions.Player.AbilitySwitch8.started -= SelectAbilityInput8;
        _inputActions.Player.AbilitySwitch9.started -= SelectAbilityInput9;
        
        _inputActions.Player.Escape.started += CancelEscInput;
    }

    private void SelectObjectInput(InputAction.CallbackContext context) => SelectObject?.Invoke();

    private void SelectPointInput(InputAction.CallbackContext context) => SelectPoint?.Invoke();

    private void SwitchTargetInput(InputAction.CallbackContext context) => SwitchTarget?.Invoke();

    private void SelectAbilityInput0(InputAction.CallbackContext context) => SelectAbility?.Invoke(0);
    private void SelectAbilityInput1(InputAction.CallbackContext context) => SelectAbility?.Invoke(1);
    private void SelectAbilityInput2(InputAction.CallbackContext context) => SelectAbility?.Invoke(2);
    private void SelectAbilityInput3(InputAction.CallbackContext context) => SelectAbility?.Invoke(3);
    private void SelectAbilityInput4(InputAction.CallbackContext context) => SelectAbility?.Invoke(4);
    private void SelectAbilityInput5(InputAction.CallbackContext context) => SelectAbility?.Invoke(5);
    private void SelectAbilityInput6(InputAction.CallbackContext context) => SelectAbility?.Invoke(6);
    private void SelectAbilityInput7(InputAction.CallbackContext context) => SelectAbility?.Invoke(7);
    private void SelectAbilityInput8(InputAction.CallbackContext context) => SelectAbility?.Invoke(8);
    private void SelectAbilityInput9(InputAction.CallbackContext context) => SelectAbility?.Invoke(9);
    
    private void CancelEscInput(InputAction.CallbackContext context) => CancelESC?.Invoke();
}

public class StackAction
{
    public event Action DefaultAction;
    private readonly List<Action> _actionsQ = new List<Action>();

    public void Invoke()
    {
        var snapshot = _actionsQ.ToArray();

        for (int i = 0; i < snapshot.Length; i++)
        {
            if(snapshot[i] != null)
            {
                snapshot[i].Invoke();
                Remove(_actionsQ[i]);
                return;
            }
        }

        DefaultAction?.Invoke();
    }

    public void Add(Action a)
    {
        _actionsQ.Insert(0, a);
    }

    public void Remove(Action a)
    {
        _actionsQ.Remove(a);
    }

    public static StackAction operator +(StackAction q, Action a)
    {
        q.Add(a);
        return q;
    }

    public static StackAction operator -(StackAction q, Action a)
    {
        q.Remove(a);
        return q;
    }
}
