using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSScript : MonoBehaviour
{
    private TMP_Text text;
    private float deltaTime = 0.0f;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        text.text = string.Format("{0:0.0} ms ({1:0} fps)", msec, fps);
    }
}
