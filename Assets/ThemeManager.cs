using UnityEngine;
using System.IO;
public class ThemeManager : MonoBehaviour
{
    //public TextAsset config;

    private void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "theme.json");

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
        ConfigThemeData configData = JsonUtility.FromJson<ConfigThemeData>(jsonString);

        foreach (ObjectData objectData in configData.objects)
        {
            GameObject prefab = Resources.Load<GameObject>(objectData.name);
            if (prefab == null)
            {
                Debug.LogError("Prefab not found: " + objectData.name);
                continue;
            }

            GameObject obj = Instantiate(prefab, objectData.position, Quaternion.Euler(objectData.rotation));
            obj.transform.localScale = objectData.scale;
        }
    }
}

[System.Serializable]
public class ConfigThemeData
{
    public ObjectData[] objects;
}

[System.Serializable]
public class ObjectData
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}
