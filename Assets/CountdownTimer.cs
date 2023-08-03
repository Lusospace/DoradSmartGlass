using System.Collections;
using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    public TMP_Text countdownText;
    public int countdownDuration = 3;

    void Start()
    {
        // Start the countdown when the script is initialized.

    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        // Show the initial countdown text (e.g., 3, 2, 1, or any other starting value).
        countdownText.text = countdownDuration.ToString();

        // Wait for a short delay before starting the countdown.
        yield return new WaitForSeconds(1f);

        // Loop through the countdown and update the text each second.
        for (int i = countdownDuration - 1; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        // Display the final countdown value (e.g., 0 or any other final value).
        countdownText.text = "Go!";

        // Wait for a short delay before hiding the countdown text.
        yield return new WaitForSeconds(1f);

        // Hide the countdown text.
        countdownText.text = "";
    }
}
