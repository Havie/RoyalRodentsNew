using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayNightTextFade : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Cycle2DDN dayNightScript;

    private void Start()
    {
        if (text)
        {
            text.color = new Color(1, 1, 1, 0);
            StartCoroutine(TriggerDisplay());
        }
        else
            Debug.LogError("DayNightPopupText not connected");
    }

    private void Update()
    {
        if (dayNightScript && text)
        {
            double fraction = dayNightScript.getCycleFraction();
            if (fraction == 0.75)
            {
                text.text = "Start of Day " + dayNightScript.getDayCount();
                StartCoroutine(TriggerDisplay());
            }
            else if (fraction == 0.25)
            {
                text.text = "Start of Night " + dayNightScript.getDayCount();
                StartCoroutine(TriggerDisplay());
            }
        }
    }

    IEnumerator TriggerDisplay()
    {
        StartCoroutine(FadeImage(false));
        yield return new WaitForSeconds(5);
        StartCoroutine(FadeImage(true));
    }

    IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                text.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                text.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }
}
