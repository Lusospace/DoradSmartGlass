using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class gpsTest : MonoBehaviour
{

    private Vector2 targetCoordinates;
    private Vector2 deviceCoordinates;
    private float distanceFromTarget = 0.0002f;
    private float proximity = 0.001f;
    private float sLatitude, sLongitude;
    public float dLatitude = 38.235406f, dLongitude = 21.768376f;
    private bool enableByRequest = true;
    public int maxWait = 10;
    public bool ready = false;
    //public Text text;
    [SerializeField]
    public TMP_Text text;

    void Start()
    {
        targetCoordinates = new Vector2(dLatitude, dLongitude);
        StartCoroutine(getLocation());

    }

    IEnumerator getLocation()
    {
        if (Input.location.isEnabledByUser)
            yield break;
        Input.location.Start();
        LocationService service = Input.location;
        if (!enableByRequest && !service.isEnabledByUser)
        {
            Debug.Log("Location Services not enabled by user");
            yield break;
        }
        service.Start(1f, 0.1f);
        while (service.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }
        if (service.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            text.text = "Target Location : " + dLatitude + ", " + dLongitude + "\nMy Location: " + service.lastData.latitude + ", " + service.lastData.longitude;
            sLatitude = service.lastData.latitude;
            sLongitude = service.lastData.longitude;
        }
        //service.Stop();
        ready = true;
        startCalculate();

    }


    public void startCalculate()
    {

        if (Input.location.status == LocationServiceStatus.Running)
        {
            deviceCoordinates = new Vector2(sLatitude, sLongitude);
            proximity = Vector2.Distance(targetCoordinates, deviceCoordinates);
            if (proximity <= distanceFromTarget)
            {
                text.text = text.text + "\nDistance : " + proximity.ToString();
                text.text += "\nTarget Detected";
                SceneManager.LoadScene("Temple");
            }
            else
            {
                text.text = text.text + "\nDistance : " + proximity.ToString();
                text.text += "\nTarget not detected, too far!";
            }
        }
        else
        {
            Debug.Log("Service Stopped");
            text.text = "Service Stopped";
        }
            


    }
}