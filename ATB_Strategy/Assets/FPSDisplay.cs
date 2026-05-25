using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fpsText;
    private float _deltaTime = 0.0f;
    private bool _show = false;

    private void OnEnable()
    {
        PlayerInputController.DebugMode += Show;
    }

    private void OnDisable()
    {
        PlayerInputController.DebugMode -= Show;
    }

    private void Show()
    {
        _show = !_show;
        
        _fpsText.gameObject.SetActive(_show);
    }

    void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.2f;
        float fps = 1.0f / _deltaTime;
        
        if (!_show) return;
        _fpsText.text = string.Format("{0:0.} FPS", fps);
    }
}
