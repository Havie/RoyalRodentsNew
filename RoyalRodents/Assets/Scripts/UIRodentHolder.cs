using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //Tell the MVC Controller which character has been Selected
        MVCController.Instance.RodentAssigned(_character);
    }
}
