using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using TMPro;
using Unity.VisualScripting;

public class KMLNavigation : MonoBehaviour
{
    private string kmlFileName; // Path to the KML file
    public GameObject player; // The player or object to move along the path
    public float moveSpeed = 5f; // Speed of movement between waypoints
    public TMP_Text navText;
    private List<Vector2> waypoints = new List<Vector2>(); // List of waypoints (2D positions)
    private int currentWaypointIndex = 0; // Index of the current waypoint
    private Vector2 currentGPSPosition; // Current GPS position from Android device (2D position)
    public RectTransform mapRect;
    public LineRenderer lineRenderer;
    public Vector2 startCoordinate, endCoordinate; // 2D positions
    public TMP_Text start, end;
    public GameObject startpoint;
    public GameObject endpoint;
    public GameObject currentpoint;
    public GameObject left, right, forward, backward;
    bool startActivity = false;

    public float scale = 10f; // Scale factor for the LineRenderer
    public Vector2 transposeVector = Vector2.zero; // Transpose vector for the LineRenderer

    void Start()
    {
        Input.location.Start();
    }

    public void StartActivity()
    {
        kmlFileName = "route2.kml";
        kmlFileName = GetKMLFilePath();
        Debug.Log(kmlFileName);
        LoadKMLData();

        // Configure LineRenderer properties
        lineRenderer.startWidth = 1f; // Adjust the line width as needed
        lineRenderer.endWidth = 1f;
        lineRenderer.useWorldSpace = false; // Set to false if you want to use local positions

        // Initialize start and end markers
        
        // Set the initial LineRenderer positions
        lineRenderer.positionCount = waypoints.Count;

        for (int i = 0; i < waypoints.Count; i++)
        {
            Vector2 scaledAndTransposedWaypoint = ScaleAndTransposeWaypoint(waypoints[i]);
            lineRenderer.SetPosition(i, new Vector3(scaledAndTransposedWaypoint.x, scaledAndTransposedWaypoint.y, 0f));
        }
        // Show the map rectangle
        ShowMapRectangle();

        // Apply initial scaling and transposing
        UpdateLineRenderer();
        if (waypoints.Count > 0)
        {
            startCoordinate = waypoints[0];
            endCoordinate = waypoints[^1];
            //start.text = startCoordinate.ToString();
            //end.text = endCoordinate.ToString();
            startpoint.transform.localPosition = startCoordinate;
            endpoint.transform.localPosition = endCoordinate;
            startpoint.SetActive(true);
            endpoint.SetActive(true);
        }
        startActivity = true;

    }
    private void UpdateRunning()
    {
        if (startActivity)
        {

            if (waypoints.Count == 0)
                return;

            // Update current GPS position from Android device
            currentGPSPosition = GetGPSPosition();
            currentpoint.transform.localPosition = currentGPSPosition;

            MoveToWaypoint();
            UpdateLineRenderer();
        }
    }
    void Update()
    {
        Invoke(nameof(UpdateRunning), 1f);
    }

    public void StopActivity()
    {
        startActivity = false;

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

    private void LoadKMLData()
    {
        // Load and parse the KML file
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(kmlFileName);

        // Retrieve the coordinates from the KML data
        XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
        nsManager.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

        // Use a Dictionary to store unique waypoints
        Dictionary<Vector2, bool> uniqueWaypoints = new Dictionary<Vector2, bool>();

        XmlNodeList coordinateNodes = xmlDoc.SelectNodes("//kml:coordinates", nsManager);
        if (coordinateNodes != null)
        {
            foreach (XmlNode node in coordinateNodes)
            {
                string[] coordinateStrings = node.InnerText.Trim().Split('\n');
                foreach (string coordinateString in coordinateStrings)
                {
                    string[] coordinate = coordinateString.Trim().Split(',');
                    float longitude, latitude;
                    if (float.TryParse(coordinate[0], out longitude) &&
                        float.TryParse(coordinate[1], out latitude))
                        
                    {
                        Vector2 waypoint = new Vector2(longitude*10, latitude*10);

                        // Check if the waypoint is unique, then add it to the dictionary
                        if (!uniqueWaypoints.ContainsKey(waypoint))
                        {
                            uniqueWaypoints.Add(waypoint, true);
                        }
                    }
                }
            }
        }

        // After processing all the waypoints, copy the unique waypoints to the main list
        waypoints.Clear();
        waypoints.AddRange(uniqueWaypoints.Keys);
    }



    private void MoveToWaypoint()
    {
        // Check if reached the current waypoint
        if (currentWaypointIndex < 0 || currentWaypointIndex >= waypoints.Count)
        {
            // Invalid waypoint index, reset it to the nearest valid index
            currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 0, waypoints.Count - 1);
            return;
        }

        Vector2 targetWaypoint = waypoints[currentWaypointIndex];
        if (Vector2.Distance(player.transform.localPosition, targetWaypoint) < 0.1f)
        {
            // Move to the next waypoint
            currentWaypointIndex++;

            // Check if reached the final waypoint
            if (currentWaypointIndex >= waypoints.Count)
            {
                // This means we have reached the end of the waypoints
                // You may want to handle this situation as needed
                Debug.Log("Reached the destination!");
                navText.text = "Reached the destination!";
                return;
            }

            // Determine navigation direction
            Vector2 nextWaypoint = waypoints[currentWaypointIndex];
            Vector2 direction = nextWaypoint - targetWaypoint;

            // Calculate angle to determine navigation action
            float angle = Vector2.SignedAngle(direction, Vector2.up);
            // Calculate distance to next waypoint
            float distanceToNextWaypoint = Vector2.Distance(player.transform.position, nextWaypoint);
            float distanceInMeters = distanceToNextWaypoint * 1000f; // Convert to meters

            // Calculate angle to current GPS position
            Vector2 gpsDirection = currentGPSPosition - targetWaypoint;
            float gpsAngle = Vector2.SignedAngle(gpsDirection, Vector2.up);

            if (distanceInMeters > 10f) // Example threshold for a certain distance in meters
            {
                Debug.Log("Keep going straight");
                navText.text = "Keep going straight";
                forward.SetActive(true);
                backward.SetActive(false);
                left.SetActive(false); 
                right.SetActive(false);
                // Add logic for going straight
            }
            // Perform navigation action based on angles
            if (gpsAngle < -45f)
            {
                Debug.Log("Turn right");
                navText.text = "Turn right";
                forward.SetActive(false);
                backward.SetActive(false);
                left.SetActive(false);
                right.SetActive(true);
                // Add logic for turning right
            }
            else if (gpsAngle > 45f)
            {
                Debug.Log("Turn left");
                navText.text = "Turn left";
                forward.SetActive(false);
                backward.SetActive(false);
                left.SetActive(true);
                right.SetActive(false);
                // Add logic for turning left
            }
            else if (angle > 30f)
            {
                Debug.Log("Go forward");
                navText.text = "Go forward";
                forward.SetActive(true);
                backward.SetActive(false);
                left.SetActive(false);
                right.SetActive(false);
                // Add logic for going forward
            }
            else if (angle < -30f)
            {
                Debug.Log("Go backward");
                navText.text = "Go backward";
                forward.SetActive(false);
                backward.SetActive(true);
                left.SetActive(false);
                right.SetActive(false);
                // Add logic for going backward
            }
        }

        // Move towards the current waypoint
        player.transform.localPosition = Vector2.MoveTowards(player.transform.localPosition, targetWaypoint, moveSpeed * Time.deltaTime);

        // Apply scaling and transposing during movement
        //UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        lineRenderer.positionCount = waypoints.Count;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (i == 0)
                transposeVector = waypoints[i];
            Vector2 scaledAndTransposedWaypoint = ScaleAndTransposeWaypoint(waypoints[i]);
            lineRenderer.SetPosition(i, new Vector3(scaledAndTransposedWaypoint.x, scaledAndTransposedWaypoint.y, 0f));
        }
    }
    private Vector2 ScaleAndTransposeWaypoint(Vector2 waypoint)
    {
        // Apply scaling and transposing to the waypoint
        Vector2 scaledAndTransposedWaypoint = (waypoint - transposeVector) * scale ;

        // Optionally, you can clamp the values to avoid extremely large coordinates
        //scaledAndTransposedWaypoint.x = Mathf.Clamp(scaledAndTransposedWaypoint.x, -1000f, 1000f);
        //scaledAndTransposedWaypoint.y = Mathf.Clamp(scaledAndTransposedWaypoint.y, -1000f, 1000f);

        return scaledAndTransposedWaypoint;
    }
    Vector2 GetGPSPosition()
    {
        Vector2 location;
        location.x = 0;
        location.y = 0;

        if (Input.location.status == LocationServiceStatus.Running)
        {
            float longitude = Input.location.lastData.longitude;
            float latitude = Input.location.lastData.latitude;

            location.x = longitude;
            location.y = latitude;
        }
        else
        {
            Input.location.Start();
            // You can handle the case when the GPS service is not running here
            // For example, display an error message or take appropriate action
        }

        return location;
    }

    private void ShowMapRectangle()
    {
        if (waypoints.Count == 0)
            return;

        // Find the bounding box of the waypoints
        float minX = waypoints[0].x;
        float minY = waypoints[0].y;
        float maxX = waypoints[0].x;
        float maxY = waypoints[0].y;

        for (int i = 1; i < waypoints.Count; i++)
        {
            minX = Mathf.Min(minX, waypoints[i].x);
            minY = Mathf.Min(minY, waypoints[i].y);
            maxX = Mathf.Max(maxX, waypoints[i].x);
            maxY = Mathf.Max(maxY, waypoints[i].y);
        }

        // Calculate the size of the bounding box
        float width = maxX - minX;
        float height = maxY - minY;

        // Set the map rectangle's position and size
        mapRect.position = new Vector3(minX, minY, 0f);
        mapRect.sizeDelta = new Vector2(width, height);
    }
}
