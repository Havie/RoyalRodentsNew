using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMoonMovement : MonoBehaviour
{
    public Cycle2DDN dayNightScript;
    public SpriteRenderer image;
    public int rotateRadius;
    public float yOffset;
    private double cycleFraction;

    // Start is called before the first frame update
    void Start()
    {
        //Should write an else that gets the daynightscript IF NULL
        if(dayNightScript)
            cycleFraction = dayNightScript.getCycleFraction();
    }

    // Update is called once per frame
    void Update()
    {
        if (dayNightScript)
            cycleFraction = dayNightScript.getCycleFraction();

        float newX, newY;
        float alpha;

        if (name == "sun")
        {
            newX = rotateRadius * Mathf.Cos(SunMoonPositionFunction(cycleFraction) + (Mathf.PI / 2));
            newY = rotateRadius * Mathf.Sin(SunMoonPositionFunction(cycleFraction) + (Mathf.PI / 2));
            alpha = SunMoonAlphaFunction(cycleFraction);
        }
        else
        {
            newX = rotateRadius * Mathf.Cos(SunMoonPositionFunction(cycleFraction) + (Mathf.PI * 3 / 2));
            newY = rotateRadius * Mathf.Sin(SunMoonPositionFunction(cycleFraction) + (Mathf.PI * 3 / 2));
            alpha = SunMoonAlphaFunction(cycleFraction + 0.5);
        }

        transform.localPosition = new Vector3(newX, newY + yOffset, 100f);
        if (image)
            image.color = new Color(1, 1, 1, alpha);
    }

    private float SunMoonPositionFunction(double time)
    {
        float theta = (-(float)time * 2 * Mathf.PI);
        return theta;
    }

    private float SunMoonAlphaFunction(double x)
    {
        float r = (1 / 2f) * Mathf.Pow(Mathf.Cos((float)x * 2 * Mathf.PI), 1 / 3f) + (1 / 2f);
        return r;
    }
}