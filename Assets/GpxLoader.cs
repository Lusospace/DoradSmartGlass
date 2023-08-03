using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class GpxLoader : MonoBehaviour
{
    public string gpxFilePath; // The path to the GPX file in the Assets folder

    void Start()
    {
        // Load the GPX file and create a new list of coordinates
        List<Vector3> coordinates = new List<Vector3>();

        XmlDocument doc = new XmlDocument();
        doc.Load(gpxFilePath);

        XmlNodeList trackpoints = doc.GetElementsByTagName("trkpt");

        foreach (XmlNode trackpoint in trackpoints)
        {
            float lat = float.Parse(trackpoint.Attributes["lat"].Value);
            float lon = float.Parse(trackpoint.Attributes["lon"].Value);

            Vector3 coordinate = new Vector3(lon, 0, lat);

            coordinates.Add(coordinate);
        }

        // Draw the coordinates as a line renderer
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = coordinates.Count;

        for (int i = 0; i < coordinates.Count; i++)
        {
            lineRenderer.SetPosition(i, coordinates[i]);
        }
    }
}
