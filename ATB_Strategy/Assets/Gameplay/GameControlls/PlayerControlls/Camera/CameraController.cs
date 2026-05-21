using System;
using UnityEngine;

[RequireComponent(typeof(TacticalCamera))]
[RequireComponent(typeof(AimCamera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private CameraMod _cameraMod;
    private TacticalCamera _tacticalCamera;
    private AimCamera _aimCamera;

    public void Init()
    {
        _tacticalCamera = GetComponent<TacticalCamera>();
        _aimCamera = GetComponentInChildren<AimCamera>();
        
        _tacticalCamera.enabled = true;
        _aimCamera.enabled = false;

        _tacticalCamera.Init();
    }

    private void OnEnable()
    {
        GameLogService.OnMessageFocus += FocusEvent;
    }

    private void OnDisable()
    {
        GameLogService.OnMessageFocus -= FocusEvent;
    }

    public void ChangeCameraMod(CameraMod cameraMod)
    {
        _cameraMod = cameraMod;

        switch (_cameraMod)
        {
            case(CameraMod.Tactical):
                _tacticalCamera.enabled = true;
                _aimCamera.enabled = false;
                break;
            case(CameraMod.Aim):
                _tacticalCamera.enabled = false;
                _aimCamera.enabled = true;
                break;
        }
    }

    public void SetExtraZoom(bool value)
    {
        ChangeCameraMod(CameraMod.Tactical);
        
        _tacticalCamera.SetExtraZoom(value);
    }

    private void FocusEvent(Transform target, int focusPriority, float focusTime)
    {
        if(_cameraMod != CameraMod.Tactical) return;
        
        _tacticalCamera.EnterFocusMode(target, focusPriority, focusTime, false);
    }

    public void FocusTarget(Transform target, int focusPriority, float focusTime = -1f,  bool instantly = false)
    {
        ChangeCameraMod(CameraMod.Tactical);
        
        _tacticalCamera.EnterFocusMode(target, focusPriority, focusTime, instantly);
    }

    public void AimTarget(Transform target, Vector3 aimPosition)
    {
        ChangeCameraMod(CameraMod.Aim);
        
        _aimCamera.SetAim(target, aimPosition);
    }
}

public enum CameraMod
{
    Tactical,
    Aim
}

///----------------------------------
///
///         Focus priority:
///         -1 = none
///         0 = simple
///         1 = better
///         5 = best
/// 
///---------------------------------