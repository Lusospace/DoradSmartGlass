using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public TMP_Text fpsText; // Reference to the Text component for displaying FPS.
    private float deltaTime = 0.0f; // Variable to keep track of time.

    private void Update()
    {
        // Calculate the frame rate and update the FPS text.
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = "FPS: " + Mathf.Ceil(fps).ToString();
    }
}
