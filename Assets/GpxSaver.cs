using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

public class GpxSaver : MonoBehaviour
{
    public string gpxFilePath; // The path to save the GPX file

    public void SaveGpx(List<Vector3> coordinates)
    {
        // Create a new XML document and add the GPX namespace and root element
        XmlDocument doc = new XmlDocument();
        XmlElement root = doc.CreateElement("gpx", "http://www.topografix.com/GPX/1/1");
        doc.AppendChild(root);

        // Add metadata to the GPX file
        XmlElement metadata = doc.CreateElement("metadata");
        root.AppendChild(metadata);

        XmlElement name = doc.CreateElement("name");
        name.InnerText = "My GPX Track";
        metadata.AppendChild(name);

        XmlElement desc = doc.CreateElement("desc");
        desc.InnerText = "A track created with Unity";
        metadata.AppendChild(desc);

        // Add the GPX track data to the GPX file
        XmlElement trk = doc.CreateElement("trk");
        root.AppendChild(trk);

        XmlElement trkseg = doc.CreateElement("trkseg");
        trk.AppendChild(trkseg);

        foreach (Vector3 coordinate in coordinates)
        {
            XmlElement trkpt = doc.CreateElement("trkpt");
            trkpt.SetAttribute("lat", coordinate.z.ToString());
            trkpt.SetAttribute("lon", coordinate.x.ToString());
            trkseg.AppendChild(trkpt);
        }

        // Save the XML document to a file
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        settings.Encoding = Encoding.UTF8;

        using (XmlWriter writer = XmlWriter.Create(gpxFilePath, settings))
        {
            doc.Save(writer);
        }

        Debug.Log("GPX file saved to: " + gpxFilePath);
    }
}
