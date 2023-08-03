using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    public GameObject avatar;
    private string action { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (action == "walk")
            Walking();
        if (action == "run")
            Running();
        if (action == "cycle")
            Running();
        else
            Walking();


    }
    public void Running()
    {
        float speed = 30.0f;
        avatar.transform.Rotate(Vector3.right * speed * Time.deltaTime);
    }
    public void Cyclling()
    {
        float speed = 45.0f;
        avatar.transform.Rotate(Vector3.right * speed * Time.deltaTime);
    }
    public void Walking()
    {
        float speed = 20.0f;
        avatar.transform.Rotate(Vector3.right * speed * Time.deltaTime);
    }
    public void SetAction(string act)
    {
        action = act;
    }
    public string getAction(string act)
    {
        return action;
    }

}
