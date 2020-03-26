using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    ExitZone _parent;

    // Start is called before the first frame update
    void Start()
    {
        _parent = this.transform.parent.GetComponent<ExitZone>();
    }

   public void imClicked()
    {

        // check time of day to see if player goes with royal guard or brings army

        print("Clicked me:" + this.gameObject);

        //get the outposts from parent 

        // show some menu that lets you select which outpost to pool from
        // outpost should show # of employees in button

    }
}
