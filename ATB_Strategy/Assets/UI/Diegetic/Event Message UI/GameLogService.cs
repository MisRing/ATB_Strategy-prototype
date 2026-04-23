using UnityEngine;

public class GameLogService : MonoBehaviour
{
    [SerializeField] private GameObject _messagePrefab;
    private static GameObject _messagePrefabInstance;
    
    [SerializeField] private CameraController _cameraController;
    private static CameraController _cameraControllerInstance;

    private void Awake()
    {
        _messagePrefabInstance = _messagePrefab;
        _cameraControllerInstance = _cameraController;
    }

    public static void ShowMessage(string message, Transform target, bool focusCamera = false)
    {
        GameObject messageObj = Instantiate(_messagePrefabInstance);
        messageObj.GetComponent<Message>().SetMessage(message, target);

        if (focusCamera)
        {
           // _cameraControllerInstance.FocusTarget();
        }
    }
}
