using UnityEngine;
using System.IO;
using TMPro;

public class ConfigReader : MonoBehaviour
{
    public TMP_Text configtxt;
    void Start()
    {
        // Get the path to the streaming assets folder
        string path = Path.Combine(Application.streamingAssetsPath, "config.json");

        // Read the contents of the file
        string jsonString = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            // On Android, the streaming assets folder is compressed in a .jar file,
            // so we need to use a WWW object to read it
            WWW reader = new WWW(path);
            while (!reader.isDone) { } // Wait for the reader to finish
            jsonString = reader.text;
        }
        else
        {
            // On other platforms, we can use a StreamReader to read the file directly
            StreamReader reader = new StreamReader(path);
            jsonString = reader.ReadToEnd();
            reader.Close();
        }

        // Parse the JSON data
        ConfigData config = JsonUtility.FromJson<ConfigData>(jsonString);

        // Do something with the config data
        Debug.Log("Config data: " + config.name);
        configtxt.text = "Config data: " + config.name;
    }
}

// Data structure to hold the config data
[System.Serializable]
public class ConfigData
{
    public string name;

    // Add more fields as needed to match your config.json file
}

