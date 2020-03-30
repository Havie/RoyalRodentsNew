using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMoonMovement : MonoBehaviour
{
    public Cycle2DDN dayNightScript;
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

        if (name == "sun")
        {
            newX = rotateRadius * Mathf.Cos(SunMoonPositionFunction(cycleFraction) + (Mathf.PI / 2));
            newY = rotateRadius * Mathf.Sin(SunMoonPositionFunction(cycleFraction) + (Mathf.PI / 2));
        }
        else
        {
            newX = rotateRadius * Mathf.Cos(SunMoonPositionFunction(cycleFraction) + (Mathf.PI * 3 / 2));
            newY = rotateRadius * Mathf.Sin(SunMoonPositionFunction(cycleFraction) + (Mathf.PI * 3 / 2));
        }

        transform.localPosition = new Vector3(newX, newY + yOffset, 100f);
    }

    private float SunMoonPositionFunction(double time)
    {
        float theta = (-(float)time * 2 * Mathf.PI);
        return theta;
    }
}