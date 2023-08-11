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

    public GameObject[] viewList;
    private Dictionary<ViewState, int> viewIndexMap = new Dictionary<ViewState, int>();
    private ViewState currentViewState;

    private void Start()
    {
        InitializeViewIndexMap();
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
        foreach (var view in viewList)
        {
            view.SetActive(false);
        }

        viewList[viewIndexMap[newState]].SetActive(true);
        currentViewState = newState;
    }

    private IEnumerator DelayedSwitchViewState(ViewState newState, float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchViewState(newState);
    }

    public void StartActivityPairingPanel()
    {
        StartCoroutine(DelayedSwitchViewState(ViewState.Pairing, 5f));
    }

    public void StartActivityDisplayPanel()
    {
        //preview widgets
        SwitchViewState(ViewState.Display);
        //StartCoroutine(DelayedSwitchViewState(ViewState.Complete, 1f));
    }
    public void StartActivityCompletePanel()
    {
        SwitchViewState(ViewState.Complete);
        //StartCoroutine(DelayedSwitchViewState(ViewState.Complete, 1f));
    }
    public void StartActivityGoodByePanel()
    {
        SwitchViewState(ViewState.GoodBye);
        //StartCoroutine(DelayedSwitchViewState(ViewState.Complete, 1f));
    }
    public void StartActivityDebugPanel()
    {
        SwitchViewState(ViewState.Debug);
        //StartCoroutine(DelayedSwitchViewState(ViewState.Complete, 1f));
    }
}
