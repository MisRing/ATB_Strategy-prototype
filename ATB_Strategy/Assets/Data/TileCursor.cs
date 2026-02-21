using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TileCursor : MonoBehaviour
{
    [Header("Main settings")]
    [SerializeField] private GameObject _spriteObject;
    [SerializeField] private Vector3 _offset = new Vector3(0, 0.1f, 0);

    public void Init()
    {
        _spriteObject.SetActive(false);
    }

    public void SetTileCursor(Vector3 tileWorldPos)
    {
        _spriteObject.SetActive(true);
        transform.position = tileWorldPos + _offset;
    }

    public void UnsetTileCursor()
    {
        _spriteObject.SetActive(false);
    }
}
