using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHitBox : MonoBehaviour
{
   

    public void OnMouseDown()
    {
        if(this.transform.parent)
        {
            Transform t = this.transform.parent;

            if (t)
            {
               // Debug.Log("Enter T");
               // No longer control clicks here, will all be done through MVC so we can pass in touch input in one single location
               // these mouse down methods will not work on Touch Screen
                if (t.GetComponent<BuildableObject>())
                {
                   // Debug.Log("Enter BO");
                   // t.GetComponent<BuildableObject>().OnMouseDown();
                }
                else if (t.GetComponent<Rodent>())
                {
                  //  Debug.Log("Enter Rodent");
                   // t.GetComponent<Rodent>().OnMouseDown();
                }
            }
        }

       // MVCController.Instance.rememberHitBox(this);
       // turnOnCollider(false);
    }

    public void turnOnCollider(bool cond)
    {
       // Debug.Log("Turn collider " + cond + " FOR:" + this.transform.gameObject);
        var _collider= this.transform.GetComponent<Collider2D>();
        _collider.enabled = cond;
    }
}
