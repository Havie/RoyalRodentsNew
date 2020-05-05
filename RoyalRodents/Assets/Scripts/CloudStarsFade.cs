using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudStarsFade : MonoBehaviour
{
    public Cycle2DDN dayNightScript;
    public SpriteRenderer image;
    private double cycleFraction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void SetUpDayNight()
    {
        if (dayNightScript == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("DayNight");
            if (go)
                dayNightScript = go.GetComponent<Cycle2DDN>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dayNightScript == null)
            SetUpDayNight();

        if (dayNightScript)
            cycleFraction = dayNightScript.getCycleFraction();
        else
            print("cant find DayNight");

        float alpha;

        if (name == "clouds")
        {
            alpha = AlphaFunction(cycleFraction);
        }
        else if (name == "stars")
        {
            alpha = AlphaFunction(cycleFraction + 0.5);
        }
        else
            alpha = 1;

        if (image)
            image.color = new Color(1, 1, 1, alpha);
    }

    private float AlphaFunction(double x)
    {
        float r = (1 / 2f) * Mathf.Pow(Mathf.Cos((float)x * 2 * Mathf.PI), 1/2f) + (1 / 2f);
        return r;
    }
}
