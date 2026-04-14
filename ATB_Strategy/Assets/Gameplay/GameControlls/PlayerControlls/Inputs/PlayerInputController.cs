using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private static InputActions _inputActions;

    public static bool IsReverseModifier { get => _inputActions.Player.ReverseInputModifier.IsPressed(); }

    public static Vector2 MouseScreenPosition { get => _inputActions.Player.MousePosition.ReadValue<Vector2>(); }

    public static Vector2 CameraMoveAxis { get => _inputActions.Camera.Move.ReadValue<Vector2>(); }
    public static float CameraZoomAxis { get => _inputActions.Camera.Zoom.ReadValue<float>(); }


    public static event Action PointLClick;
    public static event Action PointRClick;
    public static StackAction SwitchTarget = new StackAction();

    public static event Action<float> RotateCamera;


    public static event Action<int> SelectAbility;
    
    public static StackAction Cancel = new StackAction();
    
    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();

        _inputActions.Player.PointLeftClick.started += SelectObjectInput;
        _inputActions.Player.PointRightClick.started += SelectPointInput;
        _inputActions.Player.SwitchTarget.started += SwitchTargetInput;

        _inputActions.Player.AbilitySwitch1.started += SelectAbilityInput1;
        _inputActions.Player.AbilitySwitch2.started += SelectAbilityInput2;
        _inputActions.Player.AbilitySwitch3.started += SelectAbilityInput3;
        _inputActions.Player.AbilitySwitch4.started += SelectAbilityInput4;
        _inputActions.Player.AbilitySwitch5.started += SelectAbilityInput5;
        _inputActions.Player.AbilitySwitch6.started += SelectAbilityInput6;
        _inputActions.Player.AbilitySwitch7.started += SelectAbilityInput7;
        _inputActions.Player.AbilitySwitch8.started += SelectAbilityInput8;
        _inputActions.Player.AbilitySwitch9.started += SelectAbilityInput9;
        _inputActions.Player.AbilitySwitch10.started += SelectAbilityInput10;


        _inputActions.Player.Escape.started += CancelEscInput;

        _inputActions.Camera.Rotate.started += RotateCameraInput;
    }

    private void OnDisable()
    {
        _inputActions.Disable();

        _inputActions.Player.PointLeftClick.started -= SelectObjectInput;
        _inputActions.Player.PointRightClick.started -= SelectPointInput;
        _inputActions.Player.SwitchTarget.started -= SwitchTargetInput;

        _inputActions.Player.AbilitySwitch1.started -= SelectAbilityInput1;
        _inputActions.Player.AbilitySwitch2.started -= SelectAbilityInput2;
        _inputActions.Player.AbilitySwitch3.started -= SelectAbilityInput3;
        _inputActions.Player.AbilitySwitch4.started -= SelectAbilityInput4;
        _inputActions.Player.AbilitySwitch5.started -= SelectAbilityInput5;
        _inputActions.Player.AbilitySwitch6.started -= SelectAbilityInput6;
        _inputActions.Player.AbilitySwitch7.started -= SelectAbilityInput7;
        _inputActions.Player.AbilitySwitch8.started -= SelectAbilityInput8;
        _inputActions.Player.AbilitySwitch9.started -= SelectAbilityInput9;
        _inputActions.Player.AbilitySwitch10.started += SelectAbilityInput10;

        _inputActions.Player.Escape.started += CancelEscInput;

        _inputActions.Camera.Rotate.started += RotateCameraInput;
    }

    private static void SelectObjectInput(InputAction.CallbackContext context)
        => PointLClick?.Invoke();

    private static void SelectPointInput(InputAction.CallbackContext context)
        => PointRClick?.Invoke();

    private static void SwitchTargetInput(InputAction.CallbackContext context)
        => SwitchTarget?.Invoke();
    

    private static void SelectAbilityInput1(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(1);
    private static void SelectAbilityInput2(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(2);
    private static void SelectAbilityInput3(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(3);
    private static void SelectAbilityInput4(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(4);
    private static void SelectAbilityInput5(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(5);
    private static void SelectAbilityInput6(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(6);
    private static void SelectAbilityInput7(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(7);
    private static void SelectAbilityInput8(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(8);
    private static void SelectAbilityInput9(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(9);
    private static void SelectAbilityInput10(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(10);


    private static void CancelEscInput(InputAction.CallbackContext context)
        => Cancel?.Invoke();

    private static void RotateCameraInput(InputAction.CallbackContext context)
        => RotateCamera?.Invoke(context.ReadValue<float>());
}

public class StackAction
{
    public event Action DefaultAction;
    private readonly List<Action> _actionsQ = new List<Action>();

    public void Invoke()
    {
        for (int i = 0; i < _actionsQ.Count; i++)
        {
            if(_actionsQ[i] != null)
            {
                _actionsQ[i].Invoke();
                return;
            }
        }

        DefaultAction?.Invoke();
    }

    private void Add(Action a)
    {
        if (_actionsQ.Contains(a))
        {
            Remove(a);
        }
        _actionsQ.Insert(0, a);
    }

    private void Remove(Action a)
    {
        if (!_actionsQ.Contains(a)) return;
        _actionsQ.RemoveAll(x => x == a);
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
