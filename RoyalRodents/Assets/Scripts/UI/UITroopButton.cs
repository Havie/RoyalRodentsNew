using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITroopButton : MonoBehaviour
{

    public bool _confirmButton;

    // Start is called before the first frame update
    void Start()
    {
        UITroopSelection.Instance.gatherButtonChildren(this.gameObject, _confirmButton);
    }


    public void imClicked()
    {
        UITroopSelection.Instance.SelectionMade(_confirmButton);
    }
    
}
