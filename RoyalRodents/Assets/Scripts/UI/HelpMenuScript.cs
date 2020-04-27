using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMenuScript : MonoBehaviour
{
    public GameObject HelpImageReference;
    
    private void OnEnable()
    {
        HelpImageReference.SetActive(true);
    }

    private void OnDisable()
    {
        HelpImageReference.SetActive(false);
    }
}
