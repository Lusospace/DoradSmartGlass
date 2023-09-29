using System.Collections;
using UnityEngine;
using TMPro;

public class ToastMessage : MonoBehaviour
{
    private TextMeshProUGUI m_TextMeshPro;
    // Start is called before the first frame update
    void Awake()
    {
        m_TextMeshPro = GetComponent<TextMeshProUGUI>();
    }
    public void showMessage(string message, float delay)
    {
        m_TextMeshPro = GetComponent<TextMeshProUGUI>();
        m_TextMeshPro.text = message;
        m_TextMeshPro.gameObject.SetActive(true);
        StartCoroutine(ShowTimedMessage(delay));
    }

    IEnumerator ShowTimedMessage(float delay)
    {
        m_TextMeshPro.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        m_TextMeshPro.gameObject.SetActive(false);
    }    
}
