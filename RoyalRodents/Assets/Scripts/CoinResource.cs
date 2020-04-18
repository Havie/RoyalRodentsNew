using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceManagerScript;

public class CoinResource : MonoBehaviour
{
    bool active = false;

    public bool isCrown = false;
    public ResourceType _resource;
    public int _amount = 1;

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

    public void setResourceType(ResourceType type)
    {
        _resource = type;

        //update pickup sprite
        Sprite spr = Resources.Load<Sprite>(ResourceManagerScript.GetIconPath(_resource));
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer)
            renderer.sprite = spr;
    }

    public void setResourceAmount(int amnt)
    {
        _amount = amnt;
    }

    public void ImClicked()
    {
        if(isCrown)
            ResourceManagerScript.Instance.incrementCrownCount(1);
        else
            ResourceManagerScript.Instance.incrementResource(_resource, _amount);
        //To:Do Play pick up Anim ?

        Destroy(this.gameObject);
    }
}
