using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGPS : MonoBehaviour
{
    //[SerializeField] 
    private GPSData _gpsdata;
    private List<GPSData> gpslist;
    void Start()
    {
        Debug.Log(Application.persistentDataPath + "/GPSData.json");
        _gpsdata = new GPSData();
        Input.location.Start();
        
    }
    void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;
            _gpsdata.latitude = latitude.ToString();
            _gpsdata.longitude = longitude.ToString();
            gpslist.Add(_gpsdata);
        }
        //SaveIntoJson();
    }
    public void SaveIntoJson()
    {
        string gpsdata = JsonUtility.ToJson(gpslist);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/GPSData.json", gpsdata);
        
    }
    
}

    [System.Serializable]
    public class GPSData
    {
        public string longitude;
        public string latitude;
    }