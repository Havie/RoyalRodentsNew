using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinResource : MonoBehaviour
{
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

    public void ImClicked()
    {
        ResourceManagerScript.Instance.incrementResource(ResourceManagerScript.ResourceType.Shiny,1);
        //To:Do Play pick up Anim ?

        Destroy(this.gameObject);
    }
}
