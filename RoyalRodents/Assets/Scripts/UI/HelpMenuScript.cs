using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpMenuScript : MonoBehaviour
{
    public GameObject HelpImageReference;


    private void Start()
    {
        GameManager.Instance.setHelpMenu(this.gameObject);
        this.GetComponent<Image>().enabled = true;
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if(HelpImageReference)
              HelpImageReference.SetActive(true);
    }

    private void OnDisable()
    {
        if (HelpImageReference)
            HelpImageReference.SetActive(false);
    }
}
