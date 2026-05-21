using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use this if you are using TextMeshPro

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // Assign your UI Text element here
    private float deltaTime = 0.0f;

    void Update()
    {
        // Calculate the smooth delta time
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.2f;
        
        // Calculate FPS
        float fps = 1.0f / deltaTime;
        
        // Update the UI text
        fpsText.text = string.Format("{0:0.} FPS", fps);
    }
}
