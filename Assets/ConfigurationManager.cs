using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;
using Assets.Bluetooth;

public class ConfigurationManager : MonoBehaviour
{
    public CallSsd1333 CallSsd1333Object;
    public DoradBluetoothService BluetoothServiceObject;
    public LocationTracker LocationTrackerObject;
    public KMLNavigation KMLNavigationObject;
    public CountdownTimer CountdownTimerObject;
    public ViewManager viewManager;
    public List<GameObject> widgetList;
    public List<GameObject> widgetPos;
    public GameObject avatar;
    public GlassDTO glassConfig;
    public Canvas canvas;
    bool newRun = false;
    bool avatarRun = false;
    bool isSetConf = false;
    float avatarSpeed = 0f;
    private Camera snapshotCamera;
    private RenderTexture renderTexture;
    public string jsonText;
    private CommandDTO commandDTO;
    public ToastMessage toastMessage;
    private string jsonAuto = @"{
      ""Avatar"": {
        ""Active"": true,
        ""Speed"": 18.75
      },
      ""RoutesDTOs"": [
        {
          ""Latitude"": 38.70061856336034,
          ""Longitude"": -8.957381918676203
        },
        {
          ""Latitude"": 38.70671683905933,
          ""Longitude"": -8.945225024701308
        },
        {
          ""Latitude"": 38.701985630081595,
          ""Longitude"": -8.944503277546072
        },
        {
          ""Latitude"": 38.701872978433386,
          ""Longitude"": -8.940750192338834
        },
        {
          ""Latitude"": 38.71054663609023,
          ""Longitude"": -8.939162348597312
        },
        {
          ""Latitude"": 38.717755109243214,
          ""Longitude"": -8.942193686649311
        },
        {
          ""Latitude"": 38.7435419727561,
          ""Longitude"": -8.928480490699792
        },
        {
          ""Latitude"": 38.78327379379296,
          ""Longitude"": -8.880556478454272
        },
        {
          ""Latitude"": 38.925473761602376,
          ""Longitude"": -8.881999972299806
        },
        {
          ""Latitude"": 38.93692729913667,
          ""Longitude"": -8.869585920414709
        },
        {
          ""Latitude"": 38.93493556584553,
          ""Longitude"": -8.86536198145887
        }
      ],
      ""WidgetDTOs"": [
        {
          ""Name"": ""Distance"",
          ""ZPosition"": 0,
          ""GlassXPosition"": -50,
          ""GlassYPosition"": 50
        },
        {
          ""Name"": ""Speed"",
          ""ZPosition"": 0,
          ""GlassXPosition"": 42.59,
          ""GlassYPosition"": 50
        },
        {
          ""Name"": ""Altitude"",
          ""ZPosition"": 0,
          ""GlassXPosition"": -3.7,
          ""GlassYPosition"": 1.67
        }
      ],
      ""WidgetConfiguration"": true
    }";
    private string jsonManual = @"{
      ""Avatar"": {
        ""Active"": false,
        ""Speed"": 0
      },
      ""RoutesDTOs"": [],
      ""WidgetDTOs"": [
        {
          ""Name"": ""Time"",
          ""ZPosition"": 6.531645173,
          ""GlassXPosition"": -50,
          ""GlassYPosition"": 50
        },
        {
          ""Name"": ""Distance"",
          ""ZPosition"": 6.531645173,
          ""GlassXPosition"": 42.59,
          ""GlassYPosition"": 50
        },
        {
          ""Name"": ""Speed"",
          ""ZPosition"": 6.531645173,
          ""GlassXPosition"": -50,
          ""GlassYPosition"": -46.67
        },
        {
          ""Name"": ""Altitude"",
          ""ZPosition"": 6.531645173,
          ""GlassXPosition"": 42.59,
          ""GlassYPosition"": -46.67
        },
        {
          ""Name"": ""Navigation"",
          ""ZPosition"": 6.531645173,
          ""GlassXPosition"": -3.7,
          ""GlassYPosition"": 1.67
        }
      ],
      ""WidgetConfiguration"": false
    }";
    
    // Start is called before the first frame update
    void Start()
    {
        viewManager.StartActivityWelcomePanel();
        viewManager.StartActivityPairingPanel();
        // Wait for a short delay before calling StartActivityPreviewWidgets
        BluetoothServiceObject.StartReadCouroutine();
        //viewManager.StartActivityPreviewWidgets();
        StartCoroutine(BluetoothConnectionCoroutine());
        // Subscribe to the DataReceived event of the Bluetooth service
        BluetoothServiceObject.DataReceived += OnBluetoothDataReceived;
        viewManager.StartActivityDisplayPanel();
        int contrast = CallSsd1333Object.GetDisplayContrast();
        LogcatLogger.Log("Level of Contrast: "+contrast);
        CallSsd1333Object.SetDisplayContrast(150);
        Invoke(nameof(StartActivity), 5f);
        Invoke(nameof(StopActivity), 30f);
        //viewManager.StartActivityCompletePanel();
        //viewManager.StartActivityGoodByePanel();
        //viewManager.StartActivityDebugPanel();

    }

    private IEnumerator BluetoothConnectionCoroutine()
    {
        // Wait for a short delay before starting the Bluetooth connection
        yield return new WaitForSeconds(1f);

        while (true)
        {
            // Start the Bluetooth connection
            BluetoothServiceObject.CreateBluetoothConnection();

            // Yielding here will allow the coroutine to be resumed in the next frame
            yield return null;
        }
    }

    void StartConfiguration(string jsonAuto)
    {
        foreach (GameObject wlist in widgetList)
        {
            wlist.SetActive(false);
        }
        
        // Define the file name for the configuration file
        string configurationFileName = "glass_config.json";

        // Combine the file name with the persistent data path to get the full file path
        string configurationFilePath = Path.Combine(Application.persistentDataPath, configurationFileName);
        ConfigurationFileManager.SaveConfiguration(jsonAuto, configurationFilePath);
        // Load the configuration from the file
        glassConfig = ConfigurationFileManager.LoadConfiguration(configurationFilePath);

        if (glassConfig != null)
        {
            //CaptureSnapshot();

            int posCount = 0;
            foreach (WidgetDTOs widget in glassConfig.WidgetDTOs)
            {
                Debug.Log("Widget Name:" + widget.Name);
                GameObject correspondingObject = widgetList.Find(w => w.name == widget.Name);

                if (correspondingObject != null)
                {
                    // Get the index of the correspondingObject in the widgetList
                    //int index = widgetList.FindIndex(w => w.name == widget.Name);
                    //Debug.Log("Index of " + widget.Name + ": " + index);

                    // Enable the GameObject if it exists
                    //correspondingObject.transform.position = CalculateRelativeWidgetPosition((float)widget.XPosition, (float)widget.YPosition, (float)widget.ZPosition);
                    if (glassConfig.WidgetConfiguration == true)
                    {
                        correspondingObject.transform.position = widgetPos[posCount].transform.position;
                    }
                    else
                    {
                        correspondingObject.transform.position = CalculateRelativeWidgetPosition((float)widget.GlassXPosition, (float)widget.GlassYPosition, (float)widget.ZPosition);
                    }
                    correspondingObject.SetActive(true);
                    posCount++;
                }
            }
            if (glassConfig.RoutesDTOs.Count > 0)
            {
                foreach (RoutesDTOs route in glassConfig.RoutesDTOs)
                {
                    Debug.Log("Latitude:" + route.Latitude);
                    Debug.Log("Longitude:" + route.Longitude);
                }
                DrawRoutes(glassConfig.RoutesDTOs);
                newRun = false;
            }
            else
            {
                newRun = true;
            }

            if (glassConfig.Avatar.Active == true)
            {
                avatarRun = true;
                avatarSpeed = (float)glassConfig.Avatar.Speed;
                Debug.Log("Avatar Speed: " + glassConfig.Avatar.Speed);
                
            }
            isSetConf = true;
            
        }
        else
        {
            Debug.Log("Configuration file not found or invalid.");
        }
        
    }
    private Vector3 CalculateRelativeWidgetPosition(float x, float y, float z)
    {
        
        x = 132 * (x / 100);
        y = 88 * (y / 100);
        z = 300;
        Vector3 relativeCoordinate = new(x, y, z);
        return relativeCoordinate;
    }
    private void DrawRoutes(List<RoutesDTOs> routes)
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = routes.Count;

        for (int i = 0; i < routes.Count; i++)
        {
            lineRenderer.SetPosition(i, new Vector3((float)routes[i].Longitude, (float)routes[i].Latitude, 0f));
        }

        // Add any additional LineRenderer settings you need
        // For example, you can set the material and width of the line
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        // Set to face its Transform Component
        
    }
    public void SendBluetoothData(string data)
    {
        BluetoothServiceObject.SendData(data);
    }
    public void StartCommandActions(string data)
    {
        commandDTO = JsonConvert.DeserializeObject<CommandDTO>(data, new JsonSerializerSettings{});
        if (commandDTO.Command == "StartRun")
            StartActivity();
        if (commandDTO.Command == "StopRun")
            StopActivity();
        if (commandDTO.Command == "StartDebug")
            StartDebug(commandDTO.Image);
        if (commandDTO.Command == "StopDebug")
            StopDebug();

    }

    // Update is called once per frame
    void Update()
    {
        CallSsd1333Object.UpdateScreenCaptured();
        //BluetoothServiceObject.reader.ReadLine();
        // Debug.Log("Received Message" + jsonText);

    }
    void OnBluetoothDataReceived(object sender, string data)
    {
        // Handle the received data here
        // Example: Update a TextMeshPro UI element with the received data
        // yourTextMeshPro.text = data;

        Debug.Log("Received data in ConfigurationManager: " + data);
        if (data.Contains("WidgetDTOs"))
        {
            StartConfiguration(data);
            if (isSetConf)
            {
                viewManager.StartActivityDisplayPanel();  
                LogcatLogger.Log("Changed to Display view");
            }
                
        }
            
        if (data.Contains("Command"))
            StartCommandActions(data);
        
        //Start Activity without command from smartphone for now
        //StartActivity();

    }
    public void StartActivity()
    {
        toastMessage.showMessage("Activity Started", 3f);
        CountdownTimerObject.StartCountdown();
        if (newRun)
        {
            LocationTrackerObject.enabled = true;
            LocationTrackerObject.StartActivity();
        }
        else
        {
            LocationTrackerObject.SetActivity(false);
            avatar.SetActive(true);
            avatar.GetComponent<AvatarRunner>().speed = avatarSpeed;
        }
        
    }
    public void StopActivity()
    {
        toastMessage.showMessage("Activity Stopped", 3f);
        if (newRun)
        {
            LocationTrackerObject.SetActivity(false);
            LocationTrackerObject.StopActivity();
        }
        else
        {
            KMLNavigationObject.StartActivity();
        }
    }

    public void StartDebug(byte[] image)
    {
        toastMessage.showMessage("Debug Started", 3f);
        viewManager.viewList[4].SetActive(true);
        RawImage rawImage = viewManager.viewList[4].GetComponent<RawImage>();
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(image);

        if (rawImage != null)
        {
            rawImage.texture = texture;
        }
    }

    public void StopDebug()
    {
        toastMessage.showMessage("Debug Stopped", 3f);
        viewManager.viewList[4].SetActive(false);
    }

    [System.Serializable]
    public class GlassDTO
    {
        public AvatarDTO Avatar { get; set; }
        public List<RoutesDTOs> RoutesDTOs { get; set; }
        public List<WidgetDTOs> WidgetDTOs { get; set; }
        public bool WidgetConfiguration { get; set; }
        
    }

    public class AvatarDTO
    {
        public bool Active { get; set; }
        public double Speed { get; set; }
    }
    public class RoutesDTOs
    {
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
    }
    public class WidgetDTOs
    {
        public string Name { get; set; }
        public float ZPosition { get; set; }
        public float GlassXPosition { get; set; }
        public float GlassYPosition { get; set; }
    }
    public class CommandDTO
    {
        public string Command { get; set; }
        public byte[] Image { get; set; }
    }
    public class WidgetConfiguration
    {
        public bool widgetconfiguration { get; set; }
    }
    public static class ConfigurationFileManager
    {
        public static void SaveConfiguration(string jsonDataString, string filePath)
        {
            // Deserialize the JSON string to a GlassDTO object
            GlassDTO glassDTO = JsonConvert.DeserializeObject<GlassDTO>(jsonDataString, new JsonSerializerSettings
            {
                // Add any custom settings you might need, such as DateTime handling
            });
            string json = JsonConvert.SerializeObject(glassDTO, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        public static GlassDTO LoadConfiguration(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<GlassDTO>(json);
            }
            else
            {
                return null;
            }
        }
    }
    private void CaptureSnapshot()
    {
        // Create a new Camera if it doesn't exist
        if (snapshotCamera == null)
        {
            snapshotCamera = new GameObject("SnapshotCamera").AddComponent<Camera>();
            snapshotCamera.enabled = false;
        }

        // Create a new RenderTexture if it doesn't exist or if its size has changed
        int textureWidth = 1024; // Change this to the desired width of the snapshot
        int textureHeight = 768; // Change this to the desired height of the snapshot
        if (renderTexture == null || renderTexture.width != textureWidth || renderTexture.height != textureHeight)
        {
            renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
        }

        // Set the snapshotCamera parameters
        snapshotCamera.targetTexture = renderTexture;
        snapshotCamera.transform.position = gameObject.transform.position;
        snapshotCamera.transform.rotation = gameObject.transform.rotation;

        // Render the snapshot
        snapshotCamera.Render();

        // Apply the snapshot to a texture
        Texture2D snapshotTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        snapshotTexture.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
        snapshotTexture.Apply();

        // Display the snapshot on a UI element or use it as needed
        // For example, you can set the snapshot as the texture of a RawImage in a Canvas
        // or create a new Sprite using Sprite.Create() and set it on a UI Image component.
        // Here, we'll set it as the background for the canvas for demonstration purposes.
        canvas.GetComponent<Canvas>().worldCamera = snapshotCamera;
        canvas.GetComponent<Canvas>().sortingLayerName = "Default";
        canvas.GetComponent<Canvas>().sortingOrder = 0;
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        canvas.GetComponent<Canvas>().worldCamera = snapshotCamera;

        canvas.GetComponent<Canvas>().gameObject.AddComponent<RawImage>().texture = snapshotTexture;

        // Clean up the temporary objects
        snapshotCamera.targetTexture = null;
        snapshotCamera.transform.position = Vector3.zero;
        snapshotCamera.transform.rotation = Quaternion.identity;
        RenderTexture.active = null;
    }


}
