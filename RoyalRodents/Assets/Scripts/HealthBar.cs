using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Transform bar;

    
    void Awake()
    {
        bar = transform.Find("BG");
        bar = bar.transform.Find("Bar");
    }


    public void SetSize(float sizeNormalized)
    {
        if(bar)
        bar.localScale = new Vector3(sizeNormalized, 1f);
        else
            Debug.LogError("Some kind of health bar error??");
    }
}


