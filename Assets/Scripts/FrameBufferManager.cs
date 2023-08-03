using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;

public class FrameBufferManager : MonoBehaviour
{
    [DllImport("libandroid")]
    private static extern int open(string path, int flags);

    [DllImport("libandroid")]
    private static extern int ioctl(int fd, int request, int arg);

    [DllImport("libandroid")]
    private static extern int close(int fd);


    [DllImport("libEGL")]
    private static extern IntPtr eglGetCurrentDisplay();

    [DllImport("libEGL")]
    private static extern IntPtr eglGetCurrentContext();

    [DllImport("libGLESv2")]
    private static extern void glGenTextures(int n, uint[] textures);

    [DllImport("libGLESv2")]
    private static extern void glDeleteTextures(int n, uint[] textures);

    [DllImport("libGLESv2")]
    private static extern void glBindTexture(int target, uint texture);

    [DllImport("libGLESv2")]
    private static extern void glTexParameteri(int target, int pname, int param);

    [DllImport("libGLESv2")]
    private static extern void glTexImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type, IntPtr pixels);

    [DllImport("libGLESv2")]
    private static extern void glGetTexImage(int target, int level, int format, int type, IntPtr pixels);

    [DllImport("libGLESv2")]
    private static extern int glGetError();
    private uint textureId;

    // Start is called before the first frame update
    void Start()
    {
        Color[] pixels = new Color[256];
        WriteToFramebuffer(pixels, 256, 256);
        Texture2D texture = CreateTextureFromFramebuffer(16, 16);
        RawImage rawImage = FindObjectOfType<RawImage>();
        rawImage.texture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void WriteToFramebuffer(Color[] pixels, int width, int height)
    {
        // Initialize OpenGL
        Rect rect = new Rect(0, 0, width, height);
        GL.PushMatrix();
        GL.LoadOrtho();
        GL.Viewport(rect);

        // Set up the texture
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.SetPixels(pixels);
        texture.Apply();

        // Set up the material
        Material material = new Material(Shader.Find("Unlit/Texture"));
        material.mainTexture = texture;

        // Draw the texture to the framebuffer
        GL.PushMatrix();
        material.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.TexCoord2(0, 0); GL.Vertex3(0, 0, 0);
        GL.TexCoord2(0, 1); GL.Vertex3(0, 1, 0);
        GL.TexCoord2(1, 1); GL.Vertex3(1, 1, 0);
        GL.TexCoord2(1, 0); GL.Vertex3(1, 0, 0);
        GL.End();
        GL.PopMatrix();

        // Clean up
        //DestroyImmediate(texture);
        //DestroyImmediate(material);

        // Restore OpenGL
        GL.PopMatrix();
    }

    Texture2D CreateTextureFromFramebuffer(int width, int height)
    {
        // Create a new texture with the specified dimensions
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Bind the texture to the framebuffer
        RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        Graphics.SetRenderTarget(rt);

        // Render the framebuffer to the texture
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        // Clean up
        Graphics.SetRenderTarget(null);
        RenderTexture.active = null;

        return texture;
    }

}
