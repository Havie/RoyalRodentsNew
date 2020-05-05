using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRodentHolder : MonoBehaviour
{
    [SerializeField]
    private Rodent _character;

    public void setRodent(Rodent r)
    {
        _character = r;
    }
    public Rodent GetRodent()
    {
        return _character;
    }

    public void ImSelected()
    {
       // Debug.Log("TellMVC I'm selected " + _character.getName());
        //Tell the MVC Controller which character has been Selected
        MVCController.Instance.RodentAssigned(_character);
    }
    public void ImDismissed()
    {
        MVCController.Instance.RodentDismissed(_character);
    }
    /**Called by "Event Trigger Pointer Enter/Exit on Button*/
    public void MouseEnter()
    {
      // Debug.Log("HEARD ENTER");
       // MVCController.Instance.CheckClicks(false);
    }
    public void MouseExit()
    {
        //Debug.Log("HEARD EXIT");
        //MVCController.Instance.CheckClicks(true);
    }
}
