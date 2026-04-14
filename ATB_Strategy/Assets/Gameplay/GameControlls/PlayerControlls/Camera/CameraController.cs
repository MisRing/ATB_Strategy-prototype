using UnityEngine;

[RequireComponent(typeof(TacticalCamera))]
[RequireComponent(typeof(AimCamera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private CameraMod _cameraMod;
    private TacticalCamera _tacticalCamera;
    private AimCamera _aimCamera;

    private void Awake()
    {
        _tacticalCamera = GetComponent<TacticalCamera>();
        _aimCamera = GetComponentInChildren<AimCamera>();
    }

    public void Init(Transform target)
    {
        _tacticalCamera.enabled = true;
        _aimCamera.enabled = false;

        _tacticalCamera.Init(target);
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

    public void FocusTarget(Transform target, bool instantly = false)
    {
        ChangeCameraMod(CameraMod.Tactical);
        
        _tacticalCamera.EnterFocusMode(target, instantly);
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