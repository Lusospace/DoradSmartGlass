using System.IO;
using UnityEngine;
using TMPro;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class LocationTracker : MonoBehaviour
{
    private string kmlFileName;
    //private StreamWriter kmlStreamWriter;
    public TMP_Text text;
    public TMP_Text stepText;
    float latitude_pre;
    float longitude_pre;
    bool isActivityStarted = false;
    private StringBuilder kmlStringBuilder;
    int stepCount = 0;
    float stepThreshold = 1.0f; // Adjust this value based on the user's average step length
    private Vector3 previousAcceleration;
    bool isActivityStopped = false;

    void Start()
    {
        previousAcceleration = Input.acceleration;
        //StartActivity();
        Input.location.Start();
        // Check if GPS is available on the device
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS is not enabled on the device.");
            LogcatLogger.Log("GPS is not enabled on the device.");
            //return;
        }


        // Check for location permission
        latitude_pre = 0.0f;
        longitude_pre = 0.0f;
        StartKMLWriting();



    }
    void StartKMLWriting()
    {
        // Open KML file for writing
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filename = "route_" + timestamp + ".kml";
        kmlFileName = Application.persistentDataPath + "/" + filename;
        // Create the KML file template
        kmlStringBuilder = new StringBuilder();
        kmlStringBuilder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        kmlStringBuilder.AppendLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
        kmlStringBuilder.AppendLine("  <Document>");
        kmlStringBuilder.AppendLine("    <name>Route</name>");
        kmlStringBuilder.AppendLine("    <Style id=\"yellowLineGreenPoly\">");
        kmlStringBuilder.AppendLine("      <LineStyle>");
        kmlStringBuilder.AppendLine("        <color>7f00ffff</color>");
        kmlStringBuilder.AppendLine("        <width>4</width>");
        kmlStringBuilder.AppendLine("      </LineStyle>");
        kmlStringBuilder.AppendLine("      <PolyStyle>");
        kmlStringBuilder.AppendLine("        <color>7f00ff00</color>");
        kmlStringBuilder.AppendLine("      </PolyStyle>");
        kmlStringBuilder.AppendLine("    </Style>");
        kmlStringBuilder.AppendLine("    <Placemark>");
        kmlStringBuilder.AppendLine("      <name>Route</name>");
        kmlStringBuilder.AppendLine("      <styleUrl>#yellowLineGreenPoly</styleUrl>");
        kmlStringBuilder.AppendLine("      <LineString>");
        kmlStringBuilder.AppendLine("        <extrude>1</extrude>");
        kmlStringBuilder.AppendLine("        <tessellate>1</tessellate>");
        kmlStringBuilder.AppendLine("        <altitudeMode>clampToGround</altitudeMode>");
        kmlStringBuilder.AppendLine("        <coordinates>");
        kmlStringBuilder.AppendLine("        </coordinates>");
        kmlStringBuilder.AppendLine("      </LineString>");
        kmlStringBuilder.AppendLine("    </Placemark>");
        kmlStringBuilder.AppendLine("  </Document>");
        kmlStringBuilder.AppendLine("</kml>");
        try
        {
            // Write the KML file template to disk
            File.WriteAllText(kmlFileName, kmlStringBuilder.ToString());
        }
        catch (IOException ex)
        {
            Debug.LogError("Failed to write KML file template: " + ex.Message);
        }
        StartCoroutine(CountSteps());
    }
    void Update()
    { 
        // Get location
        if(isActivityStarted) { 
            if (Input.location.status == LocationServiceStatus.Running) {
                float latitude = Input.location.lastData.latitude;
                float longitude = Input.location.lastData.longitude;
                //Debug.Log("Latitude: " + latitude + ", Longitude: " + longitude);
                text.text = "Latitude: " + latitude + ", Longitude: " + longitude;
                LocationInfo location = Input.location.lastData;
                // Write KML coordinate
                if (latitude_pre != latitude || longitude_pre != longitude)
                {
                    // Update coordinates in the KML file
                    string coordinates = location.longitude + "," + location.latitude + "," + location.altitude;

                    try
                    {
                        // Read the existing KML file
                        string kmlContent = File.ReadAllText(kmlFileName);

                        // Find the opening and closing <coordinates> tags
                        int startIndex = kmlContent.IndexOf("<coordinates>") + "<coordinates>".Length;
                        int endIndex = kmlContent.IndexOf("</coordinates>");

                        // Insert the coordinates in between the tags
                        kmlContent = kmlContent.Insert(startIndex, coordinates + "\n");

                        // Write the updated KML content back to the file
                        File.WriteAllText(kmlFileName, kmlContent);
                    }
                    catch (IOException ex)
                    {
                        Debug.LogError("Failed to update KML file: " + ex.Message);
                    }
                }

                latitude_pre = latitude;
                longitude_pre = longitude;
            
            }
            else
            {
                Input.location.Start();
            }
        }
        else
        {
            if (isActivityStopped)
            {
                kmlFileName = GetKMLFilePath();
                List<RoutePoint> routePoints = LoadKMLData();
                string json = ConvertRoutePointsToJson(routePoints);
                Debug.Log(json);
                GameManager.Instance.SendBluetoothData(json);
            }
        }
    }
    private List<RoutePoint> LoadKMLData()
    {
        List<RoutePoint> routePoints = new List<RoutePoint>();

        // Load and parse the KML file
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(kmlFileName);

        // Retrieve the coordinates from the KML data
        XmlNamespaceManager nsManager = new(xmlDoc.NameTable);
        nsManager.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

        XmlNodeList coordinateNodes = xmlDoc.SelectNodes("//kml:coordinates", nsManager);
        if (coordinateNodes != null)
        {
            foreach (XmlNode node in coordinateNodes)
            {
                string[] coordinateStrings = node.InnerText.Trim().Split('\n');
                foreach (string coordinateString in coordinateStrings)
                {
                    string[] coordinate = coordinateString.Trim().Split(',');
                    if (float.TryParse(coordinate[0], out float longitude) &&
                        float.TryParse(coordinate[1], out float latitude))
                    {
                        RoutePoint waypoint = new RoutePoint
                        {
                            Longitude = longitude,
                            Latitude = latitude
                        };

                        // Check if the waypoint is unique, then add it to the list
                        if (!routePoints.Contains(waypoint))
                        {
                            routePoints.Add(waypoint);
                        }
                    }
                }
            }
        }

        return routePoints;
    }

    private string GetKMLFilePath()
    {
        string streamingAssetsPath = Application.streamingAssetsPath;
        string kmlFilePath = Path.Combine(streamingAssetsPath, kmlFileName);

        // Check if we're running on Android and use UnityWebRequest for file access
        if (Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest www = UnityWebRequest.Get(kmlFilePath);
            www.SendWebRequest();

            while (!www.isDone)
            {
                // Wait until the request is done
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load KML file: " + www.error);
                return string.Empty;
            }

            string filePath = Path.Combine(Application.persistentDataPath, kmlFileName);
            File.WriteAllBytes(filePath, www.downloadHandler.data);
            return filePath;
        }
        else
        {
            return kmlFilePath;
        }
    }
    private string ConvertRoutePointsToJson(List<RoutePoint> routePoints)
    {
        string json = JsonConvert.SerializeObject(new { RoutesDTOs = routePoints }, Newtonsoft.Json.Formatting.Indented);
        return json;
    }

    void OnApplicationQuit()
    {
        
        //kmlStreamWriter.Close();
        SetActivity(false);
        // Stop location service
        Input.location.Stop();
    }
    void OnDestroy()
    {
        OnApplicationQuit();
    }
    public bool GetActivity() { return isActivityStarted; }
    public void SetActivity(bool activity) { isActivityStarted = activity; }
    public void StartActivity()
    {
        Invoke("StartKMLWriting",3f);
        SetActivity(true);

    }
    public void StopActivity()
    {
        SetActivity(false);
        isActivityStopped = true;
    }
    IEnumerator CountSteps()
    {
        while (true)
        {
            if (isActivityStarted && Input.location.status == LocationServiceStatus.Running)
            {
                float latitude = Input.location.lastData.latitude;
                float longitude = Input.location.lastData.longitude;

                if (latitude_pre != latitude || longitude_pre != longitude)
                {
                    // Calculate distance between current and previous position
                    float distance = CalculateDistance(latitude, longitude, latitude_pre, longitude_pre);

                    // Check if a step is taken
                    if (distance >= stepThreshold)
                    {
                        stepCount++;
                        Debug.Log("Step Count: " + stepCount);
                        stepText.text = "Steps: "+stepCount.ToString();
                    }
                }

                latitude_pre = latitude;
                longitude_pre = longitude;
            }

            yield return new WaitForSeconds(1.0f); // Adjust the interval based on your needs
        }
    }

    float CalculateDistance(float lat1, float lon1, float lat2, float lon2)
    {
        float R = 6371000; // Earth's radius in meters
        float dLat = Mathf.Deg2Rad * (lat2 - lat1);
        float dLon = Mathf.Deg2Rad * (lon2 - lon1);
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(Mathf.Deg2Rad * lat1) * Mathf.Cos(Mathf.Deg2Rad * lat2) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        float d = R * c;

        return d;
    }

    public void StepCounterAccelerometer()
    {
        // Get accelerometer data
        Vector3 currentAcceleration = Input.acceleration;

        // Calculate the magnitude of the acceleration change
        float accelerationDelta = (currentAcceleration - previousAcceleration).magnitude;

        // Check if a step is taken (you might need to adjust the threshold value)
        float stepThreshold = 0.3f; // Adjust this value based on your needs
        if (accelerationDelta >= stepThreshold)
        {
            stepCount++;
            Debug.Log("Step Count: " + stepCount);
            stepText.text = "Steps: " + stepCount.ToString();
        }

        // Update the previousAcceleration to the current one for the next iteration
        previousAcceleration = currentAcceleration;
    }
}
[Serializable]
public class RoutePoint
{
    public float Latitude;
    public float Longitude;
}

