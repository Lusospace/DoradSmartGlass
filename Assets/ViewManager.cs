using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    public enum ViewState
    {
        Welcome,
        Pairing,
        Display,
        Complete,
        GoodBye,
        Debug
    }
    [SerializeField]
    public GameObject[] viewList;
    private Dictionary<ViewState, int> viewIndexMap = new Dictionary<ViewState, int>();
    private ViewState currentViewState;
    private int currentViewIndex = -1;

    private void Start()
    {
        //InitializeViewIndexMap();
        SwitchViewState(ViewState.Welcome);
    }

    private void InitializeViewIndexMap()
    {
        for (int i = 0; i < viewList.Length; i++)
        {
            viewIndexMap[(ViewState)i] = i;
        }
    }

    private void SwitchViewState(ViewState newState)
    {
        // Deactivate the current view
        if (currentViewIndex >= 0 && currentViewIndex < viewList.Length)
        {
            viewList[currentViewIndex].SetActive(false);
        }

        // Activate the new view
        currentViewIndex = (int)newState;
        if (currentViewIndex >= 0 && currentViewIndex < viewList.Length)
        {
            viewList[currentViewIndex].SetActive(true);
        }
    }

    private IEnumerator DelayedSwitchViewState(ViewState newState, float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchViewState(newState);
    }
    public void StartActivityWelcomePanel()
    {
        StartCoroutine(DelayedSwitchViewState(ViewState.Welcome, 0f));

    }
    public void StartActivityPairingPanel()
    {
        StartCoroutine(DelayedSwitchViewState(ViewState.Pairing, 5f));

    }

    public void StartActivityDisplayPanel()
    {
        //preview widgets
        //SwitchViewState(ViewState.Display);
        StartCoroutine(DelayedSwitchViewState(ViewState.Display, 0f));
    }
    public void StartActivityCompletePanel()
    {
        //SwitchViewState(ViewState.Complete);
        StartCoroutine(DelayedSwitchViewState(ViewState.Complete, 30f));
    }
    public void StartActivityGoodByePanel()
    {
        //SwitchViewState(ViewState.GoodBye);
        StartCoroutine(DelayedSwitchViewState(ViewState.GoodBye, 40f));
    }
    public void StartActivityDebugPanel()
    {
        //SwitchViewState(ViewState.Debug);
        StartCoroutine(DelayedSwitchViewState(ViewState.Debug,50f));
    }
    private GameObject FindViewByName(string viewName)
    {
        return System.Array.Find(viewList, view => view.tag == viewName);
    }
    public void EnableDisableViews(string currentView, string nextView)
    {
        // Deactivate the current view
        GameObject currentViewObject = FindViewByName(currentView);
        if (currentViewObject != null)
        {
            currentViewObject.SetActive(false);
        }

        // Activate the next view
        GameObject nextViewObject = FindViewByName(nextView);
        if (nextViewObject != null)
        {
            nextViewObject.SetActive(true);
        }
    }
}
