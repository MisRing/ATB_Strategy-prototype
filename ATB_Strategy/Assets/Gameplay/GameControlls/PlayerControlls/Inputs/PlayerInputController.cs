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

    public static event Action<int, int> SelectAbility;

    public static event Action<int> SelectUtility;

    public static StackAction Cancel = new StackAction();
    
    public static event Action DebugMode;
    
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

        _inputActions.Player.Utility1.started += SelectUtilityInput1;
        _inputActions.Player.Utility2.started += SelectUtilityInput2;
        _inputActions.Player.Utility3.started += SelectUtilityInput3;
        _inputActions.Player.Utility4.started += SelectUtilityInput4;
        _inputActions.Player.Utility5.started += SelectUtilityInput5;
        _inputActions.Player.Utility6.started += SelectUtilityInput6;
        _inputActions.Player.Utility7.started += SelectUtilityInput7;
        _inputActions.Player.Utility8.started += SelectUtilityInput8;
        _inputActions.Player.Utility9.started += SelectUtilityInput9;
        _inputActions.Player.Utility10.started += SelectUtilityInput10;
        _inputActions.Player.Utility11.started += SelectUtilityInput11;
        _inputActions.Player.Utility12.started += SelectUtilityInput12;

        _inputActions.Player.Escape.started += CancelEscInput;

        _inputActions.Camera.Rotate.started += RotateCameraInput;
        
        _inputActions.Player.DebugMode.started += DebugModeInput;
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

        _inputActions.Player.Utility1.started -= SelectUtilityInput1;
        _inputActions.Player.Utility2.started -= SelectUtilityInput2;
        _inputActions.Player.Utility3.started -= SelectUtilityInput3;
        _inputActions.Player.Utility4.started -= SelectUtilityInput4;
        _inputActions.Player.Utility5.started -= SelectUtilityInput5;
        _inputActions.Player.Utility6.started -= SelectUtilityInput6;
        _inputActions.Player.Utility7.started -= SelectUtilityInput7;
        _inputActions.Player.Utility8.started -= SelectUtilityInput8;
        _inputActions.Player.Utility9.started -= SelectUtilityInput9;
        _inputActions.Player.Utility10.started -= SelectUtilityInput10;
        _inputActions.Player.Utility11.started -= SelectUtilityInput11;
        _inputActions.Player.Utility12.started -= SelectUtilityInput12;

        _inputActions.Player.Escape.started -= CancelEscInput;

        _inputActions.Camera.Rotate.started -= RotateCameraInput;

        _inputActions.Player.DebugMode.started -= DebugModeInput;
    }

    private static void SelectObjectInput(InputAction.CallbackContext context)
        => PointLClick?.Invoke();

    private static void SelectPointInput(InputAction.CallbackContext context)
        => PointRClick?.Invoke();

    private static void SwitchTargetInput(InputAction.CallbackContext context)
        => SwitchTarget?.Invoke();
    

    private static void SelectAbilityInput1(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(1, 0);
    private static void SelectAbilityInput2(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(2, 0);
    private static void SelectAbilityInput3(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(3, 0);
    private static void SelectAbilityInput4(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(4, 0);
    private static void SelectAbilityInput5(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(5, 0);
    private static void SelectAbilityInput6(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(6, 0);
    private static void SelectAbilityInput7(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(7, 0);
    private static void SelectAbilityInput8(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(8, 0);
    private static void SelectAbilityInput9(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(9, 0);
    private static void SelectAbilityInput10(InputAction.CallbackContext context)
        => SelectAbility?.Invoke(10, 0);

    private static void SelectUtilityInput1(InputAction.CallbackContext context)
        => SelectUtility?.Invoke(0);
    private static void SelectUtilityInput2(InputAction.CallbackContext context)
        => SelectUtility?.Invoke(1);
    private static void SelectUtilityInput3(InputAction.CallbackContext context)
        => SelectUtility?.Invoke(2);
    private static void SelectUtilityInput4(InputAction.CallbackContext context)
        => SelectUtility?.Invoke(3);
    private static void SelectUtilityInput5(InputAction.CallbackContext context)
        => SelectUtility?.Invoke(4);
    private static void SelectUtilityInput6(InputAction.CallbackContext context)
        => SelectUtility?.Invoke(5);
    private static void SelectUtilityInput7(InputAction.CallbackContext context)
        => SelectUtility?.Invoke(6);
    private static void SelectUtilityInput8(InputAction.CallbackContext context)
        => SelectUtility?.Invoke(7);
    private static void SelectUtilityInput9(InputAction.CallbackContext context)
        => SelectUtility?.Invoke(8);
    private static void SelectUtilityInput10(InputAction.CallbackContext context)
        => SelectUtility?.Invoke(9);
    private static void SelectUtilityInput11(InputAction.CallbackContext context)
        => SelectUtility?.Invoke(10);
    private static void SelectUtilityInput12(InputAction.CallbackContext context)
        => SelectUtility?.Invoke(11);


    private static void CancelEscInput(InputAction.CallbackContext context)
        => Cancel?.Invoke();

    private static void RotateCameraInput(InputAction.CallbackContext context)
        => RotateCamera?.Invoke(context.ReadValue<float>());
    
    private static void DebugModeInput(InputAction.CallbackContext context)
        => DebugMode?.Invoke();
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
