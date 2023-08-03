using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoCameraSimulation : MonoBehaviour
{
    public Camera leftCamera;
    public Camera rightCamera;
    public Renderer display;

    private RenderTexture leftTexture;
    private RenderTexture rightTexture;
    private Texture2D combinedTexture;

    void Start()
    {
        // Create the RenderTextures for each camera
        leftTexture = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);
        rightTexture = new RenderTexture(1920, 1080, 24, RenderTextureFormat.ARGB32);

        // Assign the RenderTextures to the cameras
        leftCamera.targetTexture = leftTexture;
        rightCamera.targetTexture = rightTexture;

        // Create the Texture2D to hold the combined image
        combinedTexture = new Texture2D(1920, 1080, TextureFormat.RGBA32, false);
    }

    void Update()
    {
        // Render each camera to its RenderTexture
        leftCamera.Render();
        rightCamera.Render();

        // Combine the textures into a single image
        RenderTexture.active = leftTexture;
        combinedTexture.ReadPixels(new Rect(0, 0, leftTexture.width, leftTexture.height), 0, 0);
        RenderTexture.active = rightTexture;
        combinedTexture.ReadPixels(new Rect(0, 0, rightTexture.width, rightTexture.height), leftTexture.width, 0);
        RenderTexture.active = null;

        // Apply the combined texture to the display
        display.material.mainTexture = combinedTexture;
    }
}

