using System;
using System.IO;
using UnityEngine;

public class CallSsd1333 : MonoBehaviour
{
    private string logoFilePath; // Move the variable declaration here
    private string screenShotPath;
    private static Texture2D img;
    AndroidJavaObject ssd1333Service;
    
    public int captureWidth = 176;
    public int captureHeight = 176;
    private void Awake()
    {
        // Access Application.persistentDataPath in Awake
        logoFilePath = Path.Combine(Application.persistentDataPath, "image.bmp");
        screenShotPath = Path.Combine(Application.persistentDataPath, "screenshot.bmp");
    }
    void Start()
    {
        /*using var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        using var context = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        using var ssd1333Service = context.Call<AndroidJavaObject>("getSystemService", "ssd1333");
        ssd1333Service.Call("init");*/
        //byte[] logoImageBytes = LoadImageBytes(logoFilePath);
        //if (logoImageBytes != null)
        //{
            /*img = new Texture2D(2, 2); // Create a new texture
            img.LoadImage(logoImageBytes);
            int width = img.width;
            int height = img.height;
            byte[] imageData = new byte[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = img.GetPixel(x, y);
                    int r = (int)(pixelColor.r * 255);
                    int g = (int)(pixelColor.g * 255);
                    int b = (int)(pixelColor.b * 255);
                    imageData[y * width + x] = (byte)Mathf.Max(Mathf.Max(r, g), b);
                }
            }
            PassImageData(imageData, ssd1333Service);*/
            //byte[] imageDataWithoutHeader = RemoveBitmapHeader(logoImageBytes);
            //byte[] imageData16Bit = ConvertTo16BitsPerPixel(imageDataWithoutHeader);

            //PassImageData(imageDataWithoutHeader, ssd1333Service);

        /*}
        else
        {
            Debug.LogError("Failed to load logo image.");
        }*/

    }
    private byte[] LoadImageBytes(string fileName)
    {
        string imagePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(imagePath))
        {

            return File.ReadAllBytes(imagePath);
        }
        else
        {
            Debug.LogError("Image file not found: " + imagePath);
            return null;
        }
    }
    private byte[] RemoveBitmapHeader(byte[] imageBytes)
    {
        int headerSize = 54; // Assuming 24-bit uncompressed BMP header size

        if (imageBytes.Length <= headerSize)
        {
            Debug.LogError("Image file is too small to contain a header.");
            return imageBytes; // Return the original data if it's too small
        }

        int newDataLength = imageBytes.Length - headerSize;
        byte[] imageDataWithoutHeader = new byte[newDataLength];

        // Copy the image data (excluding the header) into the new array
        Array.Copy(imageBytes, headerSize, imageDataWithoutHeader, 0, newDataLength);

        return imageDataWithoutHeader;
    }


    public void PassImageData(byte[] logoImageBytes, AndroidJavaObject ssd1333Service)
    {
        
        //ssd1333Service.Call("setImage", logoImageBytes, 0);
        //ssd1333Service.Call("loadCalibrationTable", "/data/local/table.csv", 0);
        ssd1333Service.Call("setImage", logoImageBytes, 0);
        
    }
    void CaptureScreenshot()
    {
        // Capture the current frame at the desired size
        RenderTexture rt = new RenderTexture(captureWidth, captureHeight, 24);
        Camera.main.targetTexture = rt;
        Texture2D screenshotTexture = new(captureWidth, captureHeight, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenshotTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        //Destroy(rt);

        // Flip the image vertically
        screenshotTexture = FlipTextureVertical(screenshotTexture);

        // Encode the texture to a byte array in BMP format
        byte[] screenshotBytes = TextureToBMP(screenshotTexture);

        // Save the byte array as a BMP file
        //File.WriteAllBytes(screenShotPath, screenshotBytes);
        using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject context = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject ssd1333Service = context.Call<AndroidJavaObject>("getSystemService", "ssd1333"))
        {
            ssd1333Service.Call("init");
            byte[] imageDataWithoutHeader = RemoveBitmapHeader(screenshotBytes);
            //byte[] imageData16Bit = ConvertTo16BitsPerPixel(imageDataWithoutHeader);
            PassImageData(imageDataWithoutHeader, ssd1333Service);
        }
        
    }

    byte[] TextureToBMP(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        // BMP file header (14 bytes)
        byte[] header = new byte[]
        {
        0x42, 0x4D,             // "BM" identifier
        0, 0, 0, 0,             // File size (set later)
        0, 0,                   // Reserved
        0, 0,                   // Reserved
        54, 0, 0, 0             // Offset to image data
        };

        // DIB header (40 bytes)
        byte[] dibHeader = new byte[]
        {
        40, 0, 0, 0,            // DIB header size
        0, 0, 0, 0,             // Image width (set later)
        0, 0, 0, 0,             // Image height (set later)
        1, 0,                   // Number of color planes (must be 1)
        16, 0,                  // Bits per pixel (16 for R5G6B5)
        0, 0, 0, 0,             // Compression method (0 for BI_RGB)
        0, 0, 0, 0,             // Image size (set later)
        0, 0, 0, 0,             // Horizontal resolution (pixels per meter, not important)
        0, 0, 0, 0,             // Vertical resolution (pixels per meter, not important)
        0, 0, 0, 0,             // Number of colors in the palette (default)
        0, 0, 0, 0,             // Number of important colors (all, since default)
        };

        // Calculate some values
        int imageDataSize = width * height * 2; // 16 bits per pixel
        int fileSize = 54 + imageDataSize; // Header size + image data size
        byte[] fileSizeBytes = BitConverter.GetBytes(fileSize);
        byte[] widthBytes = BitConverter.GetBytes(width);
        byte[] heightBytes = BitConverter.GetBytes(height);
        byte[] imageSizeBytes = BitConverter.GetBytes(imageDataSize);

        // Update header fields
        Array.Copy(fileSizeBytes, 0, header, 2, 4);
        Array.Copy(widthBytes, 0, dibHeader, 4, 4);
        Array.Copy(heightBytes, 0, dibHeader, 8, 4);
        Array.Copy(imageSizeBytes, 0, dibHeader, 20, 4);

        // Convert the texture data to R5G6B5 format
        Color[] pixels = texture.GetPixels();
        ushort[] imageData = new ushort[width * height];
        int index = 0;
        for (int i = height - 1; i >= 0; i--) // Start from the bottom row
        {
            for (int j = 0; j < width; j++)
            {
                Color pixel = pixels[i * width + j];
                ushort r5 = (ushort)(pixel.r * 31); // 5 bits for red
                ushort g6 = (ushort)(pixel.g * 63); // 6 bits for green
                ushort b5 = (ushort)(pixel.b * 31); // 5 bits for blue
                ushort rgb565 = (ushort)((r5 << 11) | (g6 << 5) | b5);
                imageData[index++] = rgb565;
            }
        }

        // Convert ushort array to byte array
        byte[] imageDataBytes = new byte[imageData.Length * 2];
        Buffer.BlockCopy(imageData, 0, imageDataBytes, 0, imageDataBytes.Length);

        // Concatenate the header and image data
        byte[] bmpData = new byte[fileSize];
        header.CopyTo(bmpData, 0);
        dibHeader.CopyTo(bmpData, 14);
        imageDataBytes.CopyTo(bmpData, 54);

        return bmpData;
    }


    Texture2D FlipTextureVertical(Texture2D original)
    {
        Texture2D flipped = new(original.width, original.height);

        for (int x = 0; x < original.width; x++)
        {
            for (int y = 0; y < original.height; y++)
            {
                flipped.SetPixel(x, y, original.GetPixel(x, original.height - y - 1));
            }
        }

        flipped.Apply();
        return flipped;
    }
    void Update()
    {
        //Invoke(nameof(CaptureScreenshot), 0.1f);
        //CaptureScreenshot();
    }
    public void UpdateScreenCaptured()
    {
        Invoke(nameof(CaptureScreenshot), 0.1f);
    }
    public void SetDisplayContrast(int val)
    {
        using (AndroidJavaClass unityPlayerClass = new("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject context = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject ssd1333Service = context.Call<AndroidJavaObject>("getSystemService", "ssd1333"))
        {
            ssd1333Service.Call("init");
            ssd1333Service.Call("setContrast", val, 0);
        }
    }

    public int GetDisplayContrast()
    {
        
        using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject context = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject ssd1333Service = context.Call<AndroidJavaObject>("getSystemService", "ssd1333"))
        {
            int contrast = 255;
            ssd1333Service.Call("init");
            try
            {
                contrast = ssd1333Service.Call<int>("getContrast", 0);
            }
            catch (Exception ex)
            {

            }
            
            return contrast;
        }
    }


}
