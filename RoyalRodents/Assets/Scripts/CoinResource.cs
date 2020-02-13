using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinResource : MonoBehaviour
{
    int value = 1;
    bool active = false;

    public void Awake()
    {
        this.GetComponent<BoxCollider2D>().enabled = false; 
    }

    private void Start()
    {
        StartCoroutine(PickUpDelay());
        
    }
    IEnumerator PickUpDelay()
    {

        yield return new WaitForSeconds(1.5f);
        this.GetComponent<BoxCollider2D>().enabled = true;
        active = true;
    }

    public bool isActive()
    {
        return active;
    }
}
