using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demolish : MonoBehaviour
{
    public void demolish()
    {
        //Debug.Log("Heard Demolish");
        MVCController.Instance.DemolishSomething();
    }

    /**Called by "Event Trigger Pointer Enter/Exit on Button*/
    public void onMouseEnter()
    {
       // Debug.Log("HEARD ENTER");
        MVCController.Instance.CheckClicks(false);
    }
    public void OnMouseExit()
    {
       // Debug.Log("HEARD EXIT");
        MVCController.Instance.CheckClicks(true);
    }

}
