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

            if(t.GetComponent<BuildableObject>())
            {
                t.GetComponent<BuildableObject>().OnMouseDown();
            }
            else if(t.GetComponent<Rodent>())
            {
                t.GetComponent<Rodent>().OnMouseDown();
            }
        }
    }
}
