using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class KmlMapViewer : MonoBehaviour
{
    [SerializeField]
    public TextAsset kmlFile;
    public GameObject pointPrefab;
    public float elevation = 0f;

    void Start()
    {
        /*XmlDocument doc = new XmlDocument();
        doc.LoadXml(kmlFile.text);

        XmlNodeList placemarks = doc.GetElementsByTagName("Placemark");

        foreach (XmlNode placemark in placemarks)
        {
            XmlNodeList coordinatesNodes = placemark.SelectNodes("Point/coordinates");

            if (coordinatesNodes.Count > 0)
            {
                string[] coordinates = coordinatesNodes[0].InnerText.Split(',');
                float longitude = float.Parse(coordinates[0]);
                float latitude = float.Parse(coordinates[1]);

                Vector3 position = new Vector3(longitude, elevation, latitude);

                GameObject pointObject = Instantiate(pointPrefab, position, Quaternion.identity);
                pointObject.transform.parent = transform;
            }
        }*/
    }
}
