using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    public List<GameObject> viewlist;
    private int viewCount;
    // Start is called before the first frame update
    void Start()
    {
        viewCount = viewlist.Count;
        for (int i = 1; i < viewCount; i++)
        {
            viewlist[i].SetActive(false);
        }
        viewlist[0].SetActive(true);

    }
    public void StartActivityWelcomePanel()
    {
        
        StartCoroutine(WaitCoroutineWelcome());
       
    }
    public void StartActivityPreviewWidgets()
    {
        foreach (GameObject view in viewlist)
        {
            view.SetActive(false);
        }
        //viewlist[2].SetActive(true);
        StartCoroutine(WaitCoroutineDisplay());
    }
    private void SwitchViews(int activeIndex)
    {
        for (int i = 0; i < viewCount; i++)
        {
            viewlist[i].SetActive(i == activeIndex);
        }
    }
    private void DelayedSwitchViews(int activeIndex, float delay)
    {
        // Use a lambda function with Invoke to achieve delayed switching
        Invoke("SwitchViews", delay);
        StartCoroutine(DelayedSwitchViewsCoroutine(activeIndex, delay));
    }
    private IEnumerator DelayedSwitchViewsCoroutine(int activeIndex, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Switch the views after the delay
        SwitchViews(activeIndex);
    }
    IEnumerator WaitCoroutineWelcome()
    {
        
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);


        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(10);

        viewlist[0].SetActive(false);
        DelayedSwitchViews(1, 0f);
        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        
    }
    IEnumerator WaitCoroutinePairing()
    {
        
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(60);
        viewlist[1].SetActive(false);
        viewlist[2].SetActive(true);
        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        
    }
    IEnumerator WaitCoroutineDisplay()
    {
        viewlist[1].SetActive(false);
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1);
        
        DelayedSwitchViews(2, 0f);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);


    }

}
